import geopandas as gpd
import sqlite3
import numpy as np
from shapely.geometry import Point
from sklearn.neighbors import BallTree
from sklearn.preprocessing import StandardScaler
from db_setup import setup_database

# Inputs
polygon_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Watervlakken Plus2.shp"
barrier_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Barrieres.shp"
sqlite_db_path = r"c:\GITHUB\klimaatatlas\data\database.db"

# Define the list of percentiles as a customizable array of values
percentiles = [5, 25, 50, 75, 95]

# Read shapefiles
polygons_gdf = gpd.read_file(polygon_shapefile_path)
barriers_gdf = gpd.read_file(barrier_shapefile_path)

# Connect to SQLite database
conn = sqlite3.connect(sqlite_db_path)

# Create a cursor object
cur = conn.cursor()

# Set up the database (table creation, checking fields, and creating indices)
setup_database(conn)

# Clear the WQPOLYGONSTATISTICS before we start
cur.execute("DELETE FROM WQPOLYGONSTATISTICS")
conn.commit()

# Fetch unique combinations of SCENARIO and SUBSTANCE
cur.execute("""
    SELECT DISTINCT SCENARIO, SUBSTANCE
    FROM WQTIMESERIES
    WHERE DATASOURCE = 'SOBEK'
""")

unique_combinations = cur.fetchall()

for index, (scenario, substance) in enumerate(unique_combinations, start=1):
    print(f"Processing combination {index}/{len(unique_combinations)}: Scenario={scenario}, Substance={substance}")

    # Fetch location data and calculate percentiles for each location
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

    percentile_data = {}
    for location_id, data in location_data.items():
        coords = data['coords']
        values = data['values']
        percentiles_values = [np.percentile(values, p) for p in percentiles]
        percentile_data[location_id] = {'coords': coords, 'percentiles': percentiles_values}

    for feature_idx, polygon in enumerate(polygons_gdf.geometry):

        print(f"Processing feature {feature_idx}")
        point = polygon.centroid
        x, y = point.x, point.y

        # Pre-filter the points within a buffer distance
        buffer_distance = 1000  # Adjust the buffer distance as needed
        buffered_polygon = polygon.buffer(buffer_distance)
        nearby_points = [data['coords'] for data in percentile_data.values() if buffered_polygon.contains(Point(data['coords']))]
        nearby_percentiles = [data['percentiles'] for data in percentile_data.values() if buffered_polygon.contains(Point(data['coords']))]

        if len(nearby_points) > 0:
            #print(f"Processing feature {feature_idx} on {len(nearby_points)} points")
            n_neighbors = min(len(nearby_points), 3)
            nearby_coords = [np.array(point) for point in nearby_points]
            scaler = StandardScaler().fit(nearby_coords)
            nearby_points_scaled = scaler.transform(nearby_coords)
            tree = BallTree(nearby_points_scaled)
            dist, idx = tree.query(scaler.transform([[x, y]]), k=n_neighbors, return_distance=True)
            weights = 1 / dist[0]
            weight_sum = sum(weights)

            for p_idx, percentile in enumerate(percentiles):
                weighted_percentiles = [weight * nearby_percentiles[i][p_idx] for i, weight in zip(idx.flatten(), weights)]
                interpolated_percentile = sum(weighted_percentiles) / weight_sum

                cur.execute("""
                    INSERT INTO WQPOLYGONSTATISTICS
                    (DATASOURCE, SCENARIO, FEATUREIDX, SUBSTANCE, PERCENTILE, DATAVALUE)
                    VALUES (?, ?, ?, ?, ?, ?)
                """, ('SOBEK', scenario, feature_idx, substance, percentile, interpolated_percentile))

    conn.commit()

conn.close()
