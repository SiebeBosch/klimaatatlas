import sqlite3


def setup_database(conn):
    cur = conn.cursor()

    # Check if WQPOLYGONSTATISTICS table exists and create it if not
    cur.execute("""
        CREATE TABLE IF NOT EXISTS WQPOLYGONSTATISTICS (
            DATASOURCE TEXT,
            SCENARIO TEXT,
            FEATUREIDX INTEGER,
            SUBSTANCE TEXT,
            PERCENTILE NUMERIC,
            DATAVALUE REAL
        )
    """)
    conn.commit()

    # Check if required fields are present and create them if not
    cur.execute("PRAGMA table_info(WQPOLYGONSTATISTICS)")
    columns = cur.fetchall()
    column_names = [col[1] for col in columns]

    required_fields = [
        ("DATASOURCE", "TEXT"),
        ("SCENARIO", "TEXT"),
        ("FEATUREIDX", "INTEGER"),
        ("SUBSTANCE", "TEXT"),
        ("PERCENTILE", "NUMERIC"),
        ("DATAVALUE", "REAL"),
    ]

    for field, field_type in required_fields:
        if field not in column_names:
            cur.execute(f"ALTER TABLE WQPOLYGONSTATISTICS ADD COLUMN {field} {field_type}")
            conn.commit()

    # Create indices on the required fields
    cur.execute("""
        CREATE INDEX IF NOT EXISTS idx_wqpolygonstatistics_individual
        ON WQPOLYGONSTATISTICS (DATASOURCE, SCENARIO, FEATUREIDX, SUBSTANCE)
    """)
    conn.commit()

    # Create a composite index on the four fields combined
    cur.execute("""
        CREATE INDEX IF NOT EXISTS idx_wqpolygonstatistics_composite
        ON WQPOLYGONSTATISTICS (DATASOURCE, SCENARIO, FEATUREIDX, SUBSTANCE, PERCENTILE)
    """)
    conn.commit()
