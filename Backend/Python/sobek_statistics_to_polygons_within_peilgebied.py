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

# Define the list of percentiles as a customizable array of values
percentiles = [5, 25, 50, 75, 95]

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
    # Clear the WQPOLYGONSTATISTICS before we start
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
    unique_combinations = [comb for comb in unique_combinations if comb not in existing_combinations]

for index, (scenario, substance) in enumerate(unique_combinations, start=1):
    combination_idx = index
    n_combinations = len(unique_combinations)
    print(f"Processing combination {combination_idx}/{n_combinations}: Scenario={scenario}, Substance={substance}")

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

    # Create a spatial index for subcatchments
    from rtree import index
    subcatchment_index = index.Index()
    for idx, row in subcatchments_gdf.iterrows():
        subcatchment_index.insert(idx, row['geometry'].bounds)

    # Precompute subcatchment relationships
    location_to_subcatchment = {}
    for location_id, data in location_data.items():
        point_coords = data['coords']
        point = Point(point_coords)
        for idx in subcatchment_index.intersection(point.coords[0]):
            subcatchment_geometry = subcatchments_gdf.iloc[idx]['geometry']
            if subcatchment_geometry.contains(point):
                location_to_subcatchment[location_id] = subcatchments_gdf.iloc[idx]['CODE']
                break

    # Assign subcatchment codes to each point in the database
    for location_id, data in location_data.items():
        location_data[location_id]['subcatchment_code'] = location_to_subcatchment.get(location_id, None)

    # Implement a more efficient computation of percentiles 
    all_values = [data['values'] for location_id, data in location_data.items()]
    max_len = max(len(values) for values in all_values)

    padded_values = np.full((len(all_values), max_len), np.nan)
    for i, values in enumerate(all_values):
        padded_values[i, :len(values)] = values

    #all_percentiles = np.percentile(padded_values, percentiles, axis=1, interpolation='linear', keepdims=False)
    all_percentiles = np.percentile(padded_values, percentiles, axis=1, method='linear', keepdims=False)

    percentile_data = {}
    for i, (location_id, data) in enumerate(location_data.items()):
        coords = data['coords']
        values = data['values']
        subcatchment_code = data.get('subcatchment_code', None)
        percentiles_values = all_percentiles[:, i]
        percentile_data[location_id] = {'coords': coords, 'percentiles': percentiles_values, 'subcatchment_code': subcatchment_code}

    #iterate through each feature in the shapefile and determine the polygon's center
    for feature_idx, polygon in enumerate(polygons_gdf.geometry):

        print(f"Processing feature {feature_idx} for combination {combination_idx}/{n_combinations}")
        point = polygon.centroid
        x, y = point.x, point.y

        polygon_subcatchment_code = None
        for index, row in subcatchments_gdf.iterrows():
            if row['geometry'].contains(point):
                polygon_subcatchment_code = row['CODE']
                break

        buffer_distance = 1000  # Adjust the buffer distance as needed
        buffered_polygon = polygon.buffer(buffer_distance)

        nearby_points = []
        nearby_percentiles = []

        for location_id, data in percentile_data.items():
            point_subcatchment_code = data['subcatchment_code']
            point_coords = data['coords']
            point = Point(point_coords)

            if polygon_subcatchment_code == point_subcatchment_code and buffered_polygon.contains(point):
                nearby_points.append(point_coords)
                nearby_percentiles.append(data['percentiles'])

        if len(nearby_points) > 0:
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
