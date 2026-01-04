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
                                 CREATE INDEX IF NOT EXISTS idx_line_way_gist ON planet_osm_line USING GIST (way);
                                 CREATE INDEX IF NOT EXISTS idx_line_source ON planet_osm_line (source);
                                 CREATE INDEX IF NOT EXISTS idx_line_target ON planet_osm_line (target);
                                 CREATE INDEX IF NOT EXISTS idx_line_highway ON planet_osm_line (highway);
                                 """);

            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_hybrid_route(
                                     start_pt geometry(Point, 4326),
                                     end_pt geometry(Point, 4326)
                                 )
                                     RETURNS TABLE (
                                                       Seq integer,
                                                       GeoJson text,
                                                       SegmentDist float,
                                                       TotalDist float
                                                   ) AS $$
                                 DECLARE
                                     start_node integer;
                                     end_node integer;
                                     search_bbox geometry;
                                 BEGIN
                                     SELECT id INTO start_node FROM planet_osm_line_noded_vertices_pgr
                                     ORDER BY the_geom <-> start_pt LIMIT 1;
                                 
                                     SELECT id INTO end_node FROM planet_osm_line_noded_vertices_pgr
                                     ORDER BY the_geom <-> end_pt LIMIT 1;
                                 
                                     IF start_node IS NULL OR end_node IS NULL THEN
                                         RETURN;
                                     END IF;
                                 
                                     search_bbox := ST_Expand(ST_Envelope(ST_Collect(start_pt, end_pt)), 1.0);
                                 
                                     RETURN QUERY
                                         WITH route_query AS (
                                             SELECT * FROM pgr_dijkstra(
                                                     format(
                                                             'SELECT id, source, target, ST_Length(way::geography) as cost 
                                                              FROM planet_osm_line_noded 
                                                              WHERE way && %L 
                                                              -- Optional: Filter by highway type if needed
                                                              -- AND highway IS NOT NULL 
                                                              ',
                                                             search_bbox
                                                     ),
                                                     start_node,
                                                     end_node,
                                                     false
                                                           )
                                         )
                                         SELECT
                                             r.seq AS Seq,
                                             ST_AsGeoJSON(e.way)::text AS GeoJson,
                                             ST_Length(e.way::geography)::float AS SegmentDist,
                                             SUM(ST_Length(e.way::geography)::float) OVER (ORDER BY r.seq) AS TotalDist
                                         FROM route_query r
                                                  JOIN planet_osm_line_noded e ON r.edge = e.id
                                         ORDER BY r.seq;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
            
            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_complete_route(
                                     start_pt geometry(Point, 4326),
                                     end_pt geometry(Point, 4326)
                                 )
                                     RETURNS json AS $$
                                 DECLARE
                                     start_node_id integer;
                                     end_node_id integer;
                                     search_bbox geometry;
                                     result_json json;
                                 BEGIN
                                     SELECT v.id INTO start_node_id
                                     FROM planet_osm_line_noded_vertices_pgr v
                                              JOIN planet_osm_line_noded r ON (v.id = r.source OR v.id = r.target)
                                     WHERE r.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential', 'service', 'unclassified')
                                     ORDER BY v.the_geom <-> start_pt
                                     LIMIT 1;
                                 
                                     SELECT v.id INTO end_node_id
                                     FROM planet_osm_line_noded_vertices_pgr v
                                              JOIN planet_osm_line_noded r ON (v.id = r.source OR v.id = r.target)
                                     WHERE r.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential', 'service', 'unclassified')
                                     ORDER BY v.the_geom <-> end_pt
                                     LIMIT 1;
                                 
                                     IF start_node_id IS NULL OR end_node_id IS NULL THEN
                                         RETURN NULL;
                                     END IF;
                                 
                                     search_bbox := ST_Expand(ST_Envelope(ST_Collect(start_pt, end_pt)), 1.0);
                                 
                                     SELECT json_build_object(
                                                    'type', 'Feature',
                                                    'geometry', ST_AsGeoJSON(ST_MakeLine(b.way ORDER BY a.seq))::json,
                                                    'properties', json_build_object(
                                                            'distance_meters', SUM(ST_Length(b.way::geography)),
                                                            'start_id', start_node_id,
                                                            'end_id', end_node_id
                                                                  )
                                            ) INTO result_json
                                     FROM pgr_dijkstra(
                                                  format(
                                                          'SELECT id, source, target, ST_Length(way::geography) as cost 
                                                           FROM planet_osm_line_noded 
                                                           WHERE way && %L 
                                                           -- Ensure we only route on actual roads
                                                           AND highway IS NOT NULL',
                                                          search_bbox
                                                  ),
                                                  start_node_id,
                                                  end_node_id,
                                                  false
                                          ) as a
                                              JOIN planet_osm_line_noded as b ON a.edge = b.id;
                                 
                                     RETURN result_json;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);

            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION calculate_driving_distance(
                                     start_pt geometry(Point, 4326),
                                     end_pt geometry(Point, 4326)
                                 )
                                     RETURNS bigint AS $$
                                 DECLARE
                                     start_node integer;
                                     end_node integer;
                                     total_meters float;
                                     search_bbox geometry;
                                 BEGIN
                                     SELECT v.id INTO start_node
                                     FROM planet_osm_line_noded_vertices_pgr v
                                              JOIN planet_osm_line_noded e ON (v.id = e.source OR v.id = e.target)
                                     WHERE ST_DWithin(v.the_geom, start_pt, 0.01)
                                       AND e.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential', 'unclassified')
                                     ORDER BY v.the_geom <-> start_pt LIMIT 1;
                                 
                                     SELECT v.id INTO end_node
                                     FROM planet_osm_line_noded_vertices_pgr v
                                              JOIN planet_osm_line_noded e ON (v.id = e.source OR v.id = e.target)
                                     WHERE ST_DWithin(v.the_geom, end_pt, 0.01)
                                       AND e.highway IN ('motorway', 'trunk', 'primary', 'secondary', 'tertiary', 'residential', 'unclassified')
                                     ORDER BY v.the_geom <-> end_pt LIMIT 1;
                                 
                                     IF start_node IS NULL OR end_node IS NULL THEN RETURN 0; END IF;
                                 
                                     search_bbox := ST_Expand(ST_Envelope(ST_Collect(start_pt, end_pt)), 0.3);
                                 
                                     SELECT sum(cost) INTO total_meters
                                     FROM pgr_dijkstra(
                                             format(
                                                     'SELECT id, source, target, 
                                                             length_m as cost, 
                                                             length_m as reverse_cost
                                                      FROM planet_osm_line_noded 
                                                      WHERE way && %L
                                                      -- CRITICAL OPTIMIZATION: EXCLUDE non-car things
                                                      AND highway IN (''motorway'', ''motorway_link'', 
                                                                      ''trunk'', ''trunk_link'', 
                                                                      ''primary'', ''primary_link'', 
                                                                      ''secondary'', ''secondary_link'', 
                                                                      ''tertiary'', ''tertiary_link'', 
                                                                      ''residential'', ''unclassified'')',
                                                     search_bbox
                                             ),
                                             start_node,
                                             end_node,
                                             false
                                          );
                                 
                                     RETURN COALESCE(total_meters, 0)::bigint;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_hybrid_route(geometry, geometry);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_complete_route(geometry, geometry);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS calculate_driving_distance(geometry, geometry);");
        }
    }
}
