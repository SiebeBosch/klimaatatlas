import geopandas as gpd
import sqlite3
import numpy as np
from shapely.geometry import Point
from sklearn.neighbors import BallTree
from sklearn.preprocessing import StandardScaler
from db_setup import setup_database

# Inputs
polygon_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Watervlakken Plus3.shp"
subcatchments_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Peilgebied vigerend besluit.shp"
sqlite_db_path = r"c:\GITHUB\klimaatatlas\data\database.db"
delete_old_results = True

# Read shapefiles
polygons_gdf = gpd.read_file(polygon_shapefile_path)
subcatchments_gdf = gpd.read_file(subcatchments_shapefile_path)

# Connect to SQLite database
print(f"connecting to database")
conn = sqlite3.connect(sqlite_db_path)

# Create a cursor object
cur = conn.cursor()

# Set up the database (table creation, checking fields, and creating indices)
setup_database(conn)

if delete_old_results:
    print(f"removing old statistics")
    cur.execute("DELETE FROM WQPOLYGONSTATISTICS")
    conn.commit()

# Fetch unique combinations of SCENARIO and SUBSTANCE
print(f"retrieving unique combinations of scenario and substance from WQTIMESERIES")
cur.execute("""
    SELECT DISTINCT SCENARIO, SUBSTANCE
    FROM WQTIMESERIES
    WHERE DATASOURCE = 'SOBEK'
""")
unique_combinations = cur.fetchall()

if not delete_old_results:
    # Fetch existing combinations of SCENARIO and SUBSTANCE in WQPOLYGONSTATISTICS
    print(f"identifying existing combinations of scenario and substance")
    cur.execute("""
        SELECT DISTINCT SCENARIO, SUBSTANCE
        FROM WQPOLYGONSTATISTICS
    """)
    existing_combinations = set(cur.fetchall())
    # Filter unique_combinations to exclude existing_combinations
    unique_combbinations = [comb for comb in unique_combinations if comb not in existing_combinations]

for index, (scenario, substance) in enumerate(unique_combinations, start=1):
    combination_idx = index
    n_combinations = len(unique_combinations)
    print(f"Processing combination {combination_idx}/{n_combinations}: Scenario={scenario}, Substance={substance}")

    # Fetch location data
    cur.execute("""
        SELECT LOCATIONID, X, Y, DATEANDTIME, DATAVALUE
        FROM WQTIMESERIES
        WHERE DATASOURCE = 'SOBEK' AND SCENARIO = ? AND SUBSTANCE = ?
        ORDER BY LOCATIONID, DATEANDTIME
    """, (scenario, substance))

    rows = cur.fetchall()
    location_data = {}
    for row in rows:
        location_id, x, y, date_time, data_value = row
        if location_id not in location_data:
            location_data[location_id] = {'coords': (x, y), 'values': []}
        location_data[location_id]['values'].append(data_value)

    # Compute the number of days where the value exceeds 0.05 for each location
    average_days_exceeding_threshold = {}
    threshold = 0.05
    for location_id, data in location_data.items():
        values = data['values']
        days_exceeding = sum(1 for v in values if v > threshold)
        average_days_exceeding_threshold[location_id] = days_exceeding / len(values)

    # ... [Unchanged code for creating spatial index]

    # Iterate through each feature in the shapefile and determine the polygon's center
    for feature_idx, polygon in enumerate(polygons_gdf.geometry):

        # ... [Unchanged code for processing each feature]

        nearby_days_exceeding = []

        for location_id, data in location_data.items():
            point = Point(data['coords'])
            if polygon.buffer(1000).contains(point):  # Check with a buffer of 1000 units
                nearby_days_exceeding.append(average_days_exceeding_threshold[location_id])

        if len(nearby_days_exceeding) > 0:
            nearby_coords = [data['coords'] for location_id, data in location_data.items()]
            scaler = StandardScaler().fit(nearby_coords)
            nearby_points_scaled = scaler.transform(nearby_coords)
            tree = BallTree(nearby_points_scaled)
            point = polygon.centroid
            x, y = point.x, point.y
            dist, idx = tree.query(scaler.transform([[x, y]]), k=3, return_distance=True)  # 3 closest points
            weights = 1 / dist[0]
            weight_sum = sum(weights)

            weighted_days_exceeding = [weight * nearby_days_exceeding[i] for i, weight in zip(idx.flatten(), weights)]
            interpolated_days_exceeding = sum(weighted_days_exceeding) / weight_sum

            cur.execute("""
                INSERT INTO WQPOLYGONSTATISTICS
                (DATASOURCE, SCENARIO, FEATUREIDX, SUBSTANCE, DATAVALUE)
                VALUES (?, ?, ?, ?, ?)
            """, ('SOBEK', scenario, feature_idx, substance, interpolated_days_exceeding))

    conn.commit()

conn.close()
