import sqlite3

def create_composite_index(database_path):
    # Connect to the SQLite database
    conn = sqlite3.connect(database_path)

    # Create a cursor object
    cursor = conn.cursor()

    # Create the composite index on CODE, SUBSTANCE, SCENARIO, and DATEANDTIME
    cursor.execute("""
        CREATE INDEX IF NOT EXISTS idx_wq_interpolated_main_fields
        ON WQ_INTERPOLATED (CODE, SUBSTANCE, SCENARIO, DATEANDTIME);
    """)

    # Commit the changes and close the connection
    conn.commit()
    conn.close()

if __name__ == "__main__":
    database_path = r"c:\GITHUB\klimaatatlas\data\database.db"
    create_composite_index(database_path)
