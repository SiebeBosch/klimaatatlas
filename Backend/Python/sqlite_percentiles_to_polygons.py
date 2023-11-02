import sqlite3
import geopandas as gpd
import pandas as pd
from scipy.spatial import distance_matrix
import numpy as np

def load_data(db_path):
    conn = sqlite3.connect(db_path)
    query = "SELECT * FROM WQPOINTSTATISTICS"
    df = pd.read_sql(query, conn)
    conn.close()
    return df

def idw_interpolation(points, values, xi, yi):
    distances = distance_matrix([(xi, yi)], points)
    weights = 1.0 / distances
    weights /= weights.sum(axis=1)[:, np.newaxis]
    values = weights @ values
    return values[0]

def main():
    db_path = "c:\\GITHUB\\klimaatatlas\\data\\Database.db"

    #shp_path = "c:\\GITHUB\\klimaatatlas\\GIS\\Peilgebied vigerend besluit.shp"
    #result_path = "c:\\GITHUB\\klimaatatlas\\GIS\\Peilgebied_WQ_Interpolated.gpkg"

    #shp_path = "c:\GITHUB\klimaatatlas\GIS\Watervlakken_Plus4.shp"
    gdf_path = "c:\\GITHUB\\klimaatatlas\\GIS\\Watervlakken_Plus4.gpkg"
    result_path = "c:\GITHUB\klimaatatlas\GIS\Watervlakken_Plus5.gpkg"
    
    df = load_data(db_path)
    gdf = gpd.read_file(gdf_path)
    
    combinations = df[['SCENARIO', 'SUBSTANCE', 'PERCENTILE']].drop_duplicates().values
    
    for scenario, substance, percentile in combinations:

        #add the word Klimaat before scenario in order to make it a valid fieldname for sqlite
        scenariofield = f'Klimaat{scenario}'        

        # Multiplying the percentile by 100 to convert it to a percentage
        percentilefield = int(round(percentile * 100))
        
        # Replacing dashes with underscores in the substance string
        substancefield = substance.replace('-', '_')

        print(f"Processing combination {scenario},{substance},{percentile}")

        subset = df[
            (df['SCENARIO'] == scenario) & 
            (df['SUBSTANCE'] == substance) & 
            (df['PERCENTILE'] == percentile)]
        
        points = subset[['X', 'Y']].values
        values = subset['DATAVALUE'].values
        
        print(f"Writing to {scenariofield},{substancefield},{percentilefield}")
        field_name = f"{scenariofield}_{substancefield}_{percentilefield}"
        
        gdf[field_name] = gdf.geometry.apply(lambda geom: 
            idw_interpolation(points, values, geom.centroid.x, geom.centroid.y))
        
    gdf.to_file(result_path, driver="GPKG")

if __name__ == "__main__":
    main()
