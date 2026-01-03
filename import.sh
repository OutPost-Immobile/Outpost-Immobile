#!/bin/sh
set -e

echo "------------------------------------------------"
echo "STARTING PLAN B: STREAMED IMPORT (osm2pgsql)"
echo "------------------------------------------------"

# CRITICAL FIX: Export the password so osm2pgsql doesn't ask for it
export PGPASSWORD=postgres

# Run the import
osm2pgsql --create \
          --database OutpostImmobile \
          --host database \
          --username postgres \
          --latlong \
          --number-processes 4 \
          /data.pbf

echo "------------------------------------------------"
echo "IMPORT FINISHED."
echo "------------------------------------------------"

echo "Building topology..."

psql -U postgres -d OutpostImmobile -c "
ALTER TABLE planet_osm_line ADD COLUMN IF NOT EXISTS source integer;
ALTER TABLE planet_osm_line ADD COLUMN IF NOT EXISTS target integer;

SELECT pgr_createTopology(
    'planet_osm_line',  -- Table to analyze
    0.00001,            -- Tolerance (snap lines closer than ~1 meter)
    'way',              -- Geometry column name
    'osm_id',           -- ID column name
    'source',           -- Source column to fill
    'target'            -- Target column to fill
);

CREATE INDEX IF NOT EXISTS planet_osm_line_vert_idx ON planet_osm_line_vertices_pgr USING GIST (the_geom);
"

echo "done"