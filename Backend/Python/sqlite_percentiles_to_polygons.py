import argparse
import sqlite3
import geopandas as gpd
import pandas as pd
from scipy.spatial import distance_matrix
import numpy as np
from shapely.geometry import LineString

parser = argparse.ArgumentParser(description='Run IDW interpolation on water quality data.')
parser.add_argument('db_path', help='Path to the SQLite database file', default=r"c:\GITHUB\klimaatatlas\data\Database.db")
parser.add_argument('gdf_path', help='Path to the GeoDataFrame file', default=r"c:\GITHUB\klimaatatlas\GIS\Watervlakken_Plus6.gpkg")
parser.add_argument('result_path', help='Path for the output file', default=r"c:\GITHUB\klimaatatlas\GIS\Watervlakken_Plus6.gpkg")
parser.add_argument('breaklines_path', help='Path for the breaklines', default=r"c:\GITHUB\klimaatatlas\GIS\Peilgebied vigerend besluit.shp")
parser.add_argument('table_name', help='Database table', default='WQPOINTSTATISTICS')
parser.add_argument('scenario_field', help='Scenario name', default='SCENARIO')
parser.add_argument('substance_field', help='Substance name', default='SUBSTANCE')
parser.add_argument('percentile_field', help='Percentile value', default='PERCENTILE')
parser.add_argument('x_field', help='X coordinate', default='X')
parser.add_argument('y_field', help='Y coordinate', default='Y')
parser.add_argument('values_field', help='Data value', default='DATAVALUE')
args = parser.parse_args()


def load_data(db_path, table_name):
    conn = sqlite3.connect(db_path)
    query = f"SELECT * FROM {table_name}"
    df = pd.read_sql(query, conn)
    conn.close()
    return df

def load_breaklines(shapefile_path):
    return gpd.read_file(shapefile_path)

def idw_interpolation(points, values, xi, yi):
    try:
        distances = distance_matrix([(xi, yi)], points)
        # Check for any zero distances to prevent division by zero
        if np.any(distances == 0):
            return values[distances.argmin()]  # Return the value of the nearest point
        weights = 1.0 / distances
        weights /= weights.sum(axis=1)[:, np.newaxis]
        values = weights @ values
        return values[0]
    except ZeroDivisionError:
        # Handle the division by zero error
        print("Error: Division by zero occurred in IDW interpolation.")
        return None  # or some default value or action
    
def idw_interpolation_with_breaklines(points, values, xi, yi, breaklines):
    distances = distance_matrix([(xi, yi)], points)
    weights = np.zeros(distances.shape)

    for i, point in enumerate(points):
        line = LineString([point, (xi, yi)])
        if not line.crosses(breaklines.unary_union):
            distance = distances[0][i]
            if distance == 0:
                return values[i]  # Nearest point
            weights[0][i] = 1.0 / distance

    if weights.sum() == 0:
        return None  # No points available for interpolation

    weights /= weights.sum()
    return np.dot(weights, values)[0]

def main(db_path, gdf_path, result_path, breaklines_path, table_name, scenario_field, substance_field, percentile_field, x_field, y_field, values_field):
    df = load_data(db_path, table_name)
    gdf = gpd.read_file(gdf_path)
    
    breaklines = load_breaklines(breaklines_path)
    
    combinations = df[[scenario_field, substance_field, percentile_field]].drop_duplicates().values

    for scenario, substance, percentile in combinations:
        scenariofield = f'{scenario}'
        percentilefield = int(round(percentile * 100))
        substancefield = substance.replace('-', '_')
        print(f"Processing combination {scenario},{substance},{percentile}")

        subset = df[
            (df[scenario_field] == scenario) & 
            (df[substance_field] == substance) & 
            (df[percentile_field] == percentile)]
        
        points = subset[[x_field, y_field]].values
        values = subset[values_field].values
        
        print(f"Writing to {scenariofield},{substancefield},{percentilefield}")
        field_name = f"{scenariofield}_{substancefield}_{percentilefield}"
        
        #gdf[field_name] = gdf.geometry.apply(lambda geom: 
        #   idw_interpolation(points, values, geom.centroid.x, geom.centroid.y))

        gdf[field_name] = gdf.geometry.apply(lambda geom: 
           idw_interpolation_with_breaklines(points, values, geom.centroid.x, geom.centroid.y, breaklines))

            
    gdf.to_file(result_path, driver="GPKG")

if __name__ == "__main__":
    main(args.db_path, args.gdf_path, args.result_path, args.breaklines_path, args.table_name, args.scenario_field, args.substance_field, args.percentile_field, args.x_field, args.y_field, args.values_field)