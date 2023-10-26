import sqlite3
import os
import math
import geopandas as gpd

# Connect to SQLite database
db_path = r"c:\GITHUB\klimaatatlas\data\database.db"
conn = sqlite3.connect(db_path)

# Read polygon shapefile
shp_path = r"c:\GITHUB\klimaatatlas\GIS\Watervlakken Plus2.shp"
gdf = gpd.read_file(shp_path)

# Function to execute query and return a dictionary
def execute_query(query):
    cur = conn.cursor()
    cur.execute(query)
    rows = cur.fetchall()
    return {row[0]: row[1] for row in rows}

# Function to get unique climate scenarios
def get_scenarios():
    cur = conn.cursor()
    cur.execute("SELECT DISTINCT SCENARIO FROM WQPOLYGONSTATISTICS")
    return [row[0] for row in cur.fetchall()]

# Calculate eutrofication risk indicator value for each feature
def calculate_eutrofication(row, Nt_values, Pt_values):
    feature_idx = row.name
    print(f"Processing feature index: {feature_idx}")  # print statement
    feature_idx = row.name
    waterdiepte = max(row["WATERDIEPT"], 0.5)
    Nt = Nt_values.get(feature_idx, 0)
    Pt = Pt_values.get(feature_idx, 0)
    eutrophic = (Pt + Nt / 2) / waterdiepte
    if row["doodlopend"] == "ja":
        eutrophic *= 1.5
    return eutrophic

# Iterate over all unique climate scenarios
for scenario in get_scenarios():
    # Read 95 percentile concentrations of Nitrogen (Nt) and Phosphorus (Pt)
    query_Nt = f"SELECT featureidx, datavalue FROM WQPOLYGONSTATISTICS WHERE substance='Nt' AND percentile=95 AND DATASOURCE='SOBEK' AND SCENARIO='{scenario}';"
    query_Pt = f"SELECT featureidx, datavalue FROM WQPOLYGONSTATISTICS WHERE substance='Pt' AND percentile=95 AND DATASOURCE='SOBEK' AND SCENARIO='{scenario}';"
    Nt_values = execute_query(query_Nt)
    Pt_values = execute_query(query_Pt)
    
    # Calculate eutrophication and write to a column named after the scenario
    gdf[scenario] = gdf.apply(calculate_eutrofication, axis=1, args=(Nt_values, Pt_values))

# Write the result back to the polygon shapefile
gdf.to_file(shp_path)

# Close SQLite database connection
conn.close()
