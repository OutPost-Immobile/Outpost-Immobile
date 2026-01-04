#!/bin/sh
set -e

echo "------------------------------------------------"
echo " STREAMED IMPORT (osm2pgsql)"
echo "------------------------------------------------"

if command -v apt-get >/dev/null; then
    apt-get update && apt-get install -y postgresql-client
elif command -v apk >/dev/null; then
    apk add --no-cache postgresql-client
else
    echo "ERROR: Could not install psql. Unknown OS."
    exit 1
fi

echo "Waiting for database to be ready..."
until pg_isready -h database -U postgres; do
  echo "Database not ready yet... sleeping 2s"
  sleep 2
done
echo "Database is ready!"

export PGPASSWORD=postgres

osm2pgsql --create \
          --database OutpostImmobile \
          --host database \
          --username postgres \
          --latlong \
          --slim \
          --number-processes 4 \
          /data.pbf

echo "------------------------------------------------"
echo "IMPORT FINISHED."
echo "------------------------------------------------"

echo "Fixing Graph Disconnections & Building Topology..."
echo "(This step may take several minutes for large maps)"

psql -h database -U postgres -d OutpostImmobile -c "
ALTER TABLE planet_osm_line DROP COLUMN IF EXISTS gid;
ALTER TABLE planet_osm_line ADD COLUMN gid SERIAL PRIMARY KEY;

SELECT pgr_nodeNetwork('planet_osm_line', 0.0001, 'gid', 'way');

SELECT pgr_createTopology('planet_osm_line_noded', 0.0001, 'geom', 'id');

ALTER TABLE planet_osm_line_noded ADD COLUMN IF NOT EXISTS highway text;

UPDATE planet_osm_line_noded AS new
SET highway = old.highway
FROM planet_osm_line AS old
WHERE new.old_id = old.gid;

CREATE INDEX IF NOT EXISTS noded_geom_idx ON planet_osm_line_noded USING GIST (geom);

SELECT pgr_analyzeGraph('planet_osm_line_noded', 0.0001, 'geom', 'id');
"

echo "------------------------------------------------"
echo "DONE. The Noded Routing Graph is ready."
echo "------------------------------------------------"