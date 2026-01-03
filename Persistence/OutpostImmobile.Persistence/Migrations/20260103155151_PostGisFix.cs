using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PostGisFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 ALTER TABLE planet_osm_line ADD COLUMN IF NOT EXISTS gid SERIAL;
                                 """);
            
            migrationBuilder.Sql("""
                                 ALTER TABLE planet_osm_line DROP CONSTRAINT IF EXISTS planet_osm_line_pkey;
                                 """);
            
            migrationBuilder.Sql("""
                                 ALTER TABLE planet_osm_line ADD PRIMARY KEY (gid);
                                 """);
            
            migrationBuilder.Sql("""
                                 CREATE INDEX IF NOT EXISTS planet_osm_line_gix ON planet_osm_line USING GIST (way);
                                 """);

            migrationBuilder.Sql("""
                                 CREATE INDEX IF NOT EXISTS idx_line_way_gist ON planet_osm_line USING GIST (way);
                                 CREATE INDEX IF NOT EXISTS idx_line_source ON planet_osm_line (source);
                                 CREATE INDEX IF NOT EXISTS idx_line_target ON planet_osm_line (target);
                                 CREATE INDEX IF NOT EXISTS idx_line_highway ON planet_osm_line (highway);
                                 """);

            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_hybrid_route(start_pt geometry, end_pt geometry)
                                 RETURNS TABLE (
                                                       seq integer,
                                                       geo_json text,
                                                       segment_dist float,
                                                       total_dist float
                                                   ) AS $$
                                 DECLARE
                                     start_node integer;
                                     end_node integer;
                                     search_bbox geometry;
                                     buffer_degree float := 0.5;
                                 BEGIN
                                     search_bbox := ST_Expand(ST_Envelope(ST_MakeLine(start_pt, end_pt)), buffer_degree);
                                 
                                     SELECT v.id INTO start_node
                                     FROM planet_osm_line_vertices_pgr v
                                              JOIN planet_osm_line r ON (v.id = r.source OR v.id = r.target)
                                     WHERE v.the_geom && ST_Expand(start_pt, 0.01)
                                       AND ST_DWithin(v.the_geom, start_pt, 0.01)
                                       AND r.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential')
                                     ORDER BY v.the_geom <-> start_pt LIMIT 1;
                                 
                                     SELECT v.id INTO end_node
                                     FROM planet_osm_line_vertices_pgr v
                                              JOIN planet_osm_line r ON (v.id = r.source OR v.id = r.target)
                                     WHERE v.the_geom && ST_Expand(end_pt, 0.01)
                                       AND ST_DWithin(v.the_geom, end_pt, 0.01)
                                       AND r.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential')
                                     ORDER BY v.the_geom <-> end_pt LIMIT 1;
                                 
                                     RETURN QUERY
                                         SELECT
                                             a.seq,
                                             ST_AsGeoJSON(b.way)::text,
                                             ST_Length(b.way::geography)::float,
                                             SUM(ST_Length(b.way::geography)::float) OVER ()
                                         FROM pgr_astar(
                                                      format(
                                                              'SELECT gid as id, source, target, 
                                                                      st_length(way) as cost, 
                                                                      -- We calculate coordinates dynamically because columns x1/y1 might not exist
                                                                      ST_X(ST_StartPoint(way)) as x1, ST_Y(ST_StartPoint(way)) as y1,
                                                                      ST_X(ST_EndPoint(way)) as x2,   ST_Y(ST_EndPoint(way)) as y2,
                                                                      st_length(way) as reverse_cost 
                                                               FROM planet_osm_line 
                                                               WHERE way && %L::geometry 
                                                               AND highway IS NOT NULL',
                                                              search_bbox
                                                      ),
                                                      start_node, end_node, false
                                              ) as a
                                                  JOIN planet_osm_line as b ON a.edge = b.gid
                                         ORDER BY a.seq;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
            
            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_complete_route(start_pt geometry, end_pt geometry)
                                 RETURNS json AS $$
                                 DECLARE
                                     start_node integer;
                                     end_node integer;
                                     result json;
                                 BEGIN
                                     -- Reuse logic or copy snapping logic here (abbreviated for clarity)
                                     SELECT v.id INTO start_node FROM planet_osm_line_vertices_pgr v
                                     ORDER BY v.the_geom <-> start_pt LIMIT 1;
                                 
                                     SELECT v.id INTO end_node FROM planet_osm_line_vertices_pgr v
                                     ORDER BY v.the_geom <-> end_pt LIMIT 1;
                                 
                                     SELECT json_build_object(
                                         'type', 'Feature',
                                         'geometry', ST_AsGeoJSON(ST_MakeLine(b.way ORDER BY a.seq))::json,
                                         'properties', json_build_object(
                                             'distance_meters', SUM(ST_Length(b.way::geography))
                                         )
                                     ) INTO result
                                     FROM pgr_dijkstra(
                                         'SELECT gid as id, source, target, st_length(way) as cost FROM planet_osm_line', 
                                         start_node, end_node, false
                                     ) as a
                                     JOIN planet_osm_line as b ON a.edge = b.gid;
                                 
                                     RETURN result;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_hybrid_route(geometry, geometry);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_complete_route(geometry, geometry);");
        }
    }
}
