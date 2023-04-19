
import geopandas as gpd
import pandas as pd
import numpy as np
import sqlite3
from scipy.spatial import distance_matrix
from scipy.spatial.distance import cdist
from rtree import index

print("Script started")

# Function for inverse distance weighting
def idw_interpolation(points, values, query_points, power=2):
    dist_matrix = cdist(query_points, points)
    weights = 1 / (dist_matrix ** power)
    weights[dist_matrix == 0] = 0
    weights_sum = weights.sum(axis=1)
    weights_sum[weights_sum == 0] = 1
    interpolated_values = (weights * values).sum(axis=1) / weights_sum
    return interpolated_values

# Read the shapefile
water_areas = gpd.read_file(r"c:\GITHUB\klimaatatlas\GIS\Watervlakken Plus2.shp")
print(f"Water surface areas shapefile loaded with {len(water_areas)} features")

# Read the SQLite database
conn = sqlite3.connect(r"c:\GITHUB\klimaatatlas\data\database.db")
query = '''
        SELECT LOCATIONID, X, Y, DATEANDTIME, SCENARIO, SUBSTANCE, DATAVALUE
        FROM WQTIMESERIES
        WHERE DATASOURCE = "SOBEK"
        '''
wq_timeseries = pd.read_sql_query(query, conn)


cursor = conn.cursor()
cursor.execute('DELETE FROM WQ_INTERPOLATED')
conn.commit()

conn.close()

# Extract unique parameters, scenarios, and timesteps
unique_params = wq_timeseries["SUBSTANCE"].unique()
unique_scenarios = wq_timeseries["SCENARIO"].unique()
unique_timesteps = wq_timeseries["DATEANDTIME"].unique()

print(f"Identified {len(unique_params)}  params")
print(f"Identified {len(unique_scenarios)}  scenarios")
print(f"Identified {len(unique_timesteps)}  timesteps")


# Extract the coordinates of the water surface areas
#area_coords = water_areas.geometry.centroid.to_numpy()
area_coords_list = [np.array([point.x, point.y]) for point in water_areas.geometry.centroid]
area_coords = np.array(area_coords_list)

print(f"area_coords are {area_coords}")

# Initialize an R-tree index for spatial queries
idx = index.Index()
for i, row in water_areas.iterrows():
    idx.insert(i, row.geometry.bounds)

# Loop through each parameter, scenario, and timestep
for param in unique_params:
    for scenario in unique_scenarios:
        for timestep in unique_timesteps:

            print(f"Processing {param} {scenario} {timestep}")

            # Extract data for the current timestep, scenario, and parameter
            data = wq_timeseries[(wq_timeseries["SUBSTANCE"] == param) &
                                 (wq_timeseries["SCENARIO"] == scenario) &
                                 (wq_timeseries["DATEANDTIME"] == timestep)]
            
            # Extract the coordinates and values of the data points
            points = data[["X", "Y"]].to_numpy()
            values = data["DATAVALUE"].to_numpy()

            # Perform IDW interpolation for the water surface areas
            interpolated_values = idw_interpolation(points, values, area_coords)

            # Store the interpolated values in a new DataFrame
            # Store the interpolated values in a new DataFrame
            interpolated_df = pd.DataFrame({
                "CODE": water_areas["CODE"],
                "DATEANDTIME": timestep,
                "SCENARIO": scenario,
                "SUBSTANCE": param,
                "DATAVALUE": interpolated_values
            })

            # Save the interpolated data to a new table in the SQLite database
            with sqlite3.connect(r"c:\GITHUB\klimaatatlas\data\database.db") as conn:
                interpolated_df.to_sql("WQ_INTERPOLATED", conn, if_exists="append", index=False)

print(f"Execution complete")