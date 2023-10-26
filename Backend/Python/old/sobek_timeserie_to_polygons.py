import geopandas as gpd
import sqlite3
from shapely.geometry import Point
from sklearn.neighbors import BallTree
from sklearn.preprocessing import StandardScaler
import numpy as np

# Inputs
polygon_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Watervlakken Plus2.shp"
barrier_shapefile_path = r"c:\GITHUB\klimaatatlas\GIS\Barrieres.shp"
sqlite_db_path = r"c:\GITHUB\klimaatatlas\data\database.db"

# Read shapefiles
polygons_gdf = gpd.read_file(polygon_shapefile_path)
barriers_gdf = gpd.read_file(barrier_shapefile_path)

# Connect to SQLite database
conn = sqlite3.connect(sqlite_db_path)
cur = conn.cursor()

# ... (Create table and indices)

# Clear the WQPOLYGONSERIES before we start
cur.execute("DELETE FROM WQPOLYGONSERIES")
conn.commit()

# Fetch unique combinations of SCENARIO, SUBSTANCE, and DATEANDTIME
cur.execute("""
    SELECT DISTINCT SCENARIO, SUBSTANCE, DATEANDTIME as DATEANDTIME
    FROM WQTIMESERIES
    WHERE DATASOURCE = 'SOBEK'
""")

unique_combinations = cur.fetchall()

# Process data
for index, (scenario, substance, date_time) in enumerate(unique_combinations, start=1):
    print(f"Processing combination {index}/{len(unique_combinations)}: Scenario={scenario}, Substance={substance}, DateTime={date_time}")
    cur.execute("""
        SELECT LOCATIONID, X, Y, DATAVALUE
        FROM WQTIMESERIES
        WHERE DATASOURCE = 'SOBEK' AND SCENARIO = ? AND SUBSTANCE = ? AND DATEANDTIME = ?
    """, (scenario, substance, date_time))

    rows = cur.fetchall()
    print(f"Number of rows is {len(rows)}")

    location_points = {}
    location_values = {}

    for row in rows:
        location_id, x, y, data_value = row
        point = Point(x, y)

        if location_id not in location_points:
            location_points[location_id] = []
            location_values[location_id] = []

        location_points[location_id].append((x, y))
        location_values[location_id].append(data_value)

    all_points = [Point(x, y) for coords in location_points.values() for x, y in coords]
    all_values = [value for values in location_values.values() for value in values]

    for feature_idx, polygon in enumerate(polygons_gdf.geometry):
        print(f"Processing feature {feature_idx}")
        point = polygon.centroid
        x, y = point.x, point.y

        # Pre-filter the points within a buffer distance
        buffer_distance = 1000  # Adjust the buffer distance as needed
        buffered_polygon = polygon.buffer(buffer_distance)
        nearby_points = [p for p in all_points if buffered_polygon.contains(p)]
        nearby_values = [v for p, v in zip(all_points, all_values) if buffered_polygon.contains(p)]

        if len(nearby_points) > 0:
            n_neighbors = min(len(nearby_points), 3)
            nearby_coords = [np.array((point.x, point.y)) for point in nearby_points]
            scaler = StandardScaler().fit(nearby_coords)
            nearby_points_scaled = scaler.transform(nearby_coords)
            tree = BallTree(nearby_points_scaled)
            dist, idx = tree.query(scaler.transform([[x, y]]), k=n_neighbors, return_distance=True)
            weights = 1 / dist[0]
            weight_sum = sum(weights)
            flattened_idx = idx.flatten()
            weighted_values = [weight * nearby_values[i] for i, weight in zip(idx.flatten(), weights)]
            interpolated_value = sum(weighted_values) / weight_sum

            cur.execute("""
                INSERT INTO WQPOLYGONSERIES
                (DATASOURCE, SCENARIO, FEATUREIDX, SUBSTANCE, DATEANDTIME, DATAVALUE)
                VALUES (?, ?, ?, ?, ?, ?)
            """, ('SOBEK', scenario, feature_idx, substance, date_time, interpolated_value))

    conn.commit()

conn.close()
