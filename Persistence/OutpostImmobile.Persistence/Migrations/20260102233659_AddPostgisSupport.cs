using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostgisSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_route_segments(
                                     start_pt geometry, 
                                     end_pt geometry
                                 )
                                 RETURNS TABLE (
                                     seq integer,
                                     geo_json text,
                                     distance_meters float
                                 ) AS $$
                                 DECLARE
                                     start_node_id integer;
                                     end_node_id integer;
                                 BEGIN
                                     -- 1. Find Start Node using the passed Geometry object
                                     -- Note: We assume the input point already has SRID 4326
                                     SELECT id INTO start_node_id 
                                     FROM planet_osm_line_vertices_pgr 
                                     ORDER BY the_geom <-> start_pt 
                                     LIMIT 1;
                                 
                                     -- 2. Find End Node
                                     SELECT id INTO end_node_id 
                                     FROM planet_osm_line_vertices_pgr 
                                     ORDER BY the_geom <-> end_pt 
                                     LIMIT 1;
                                 
                                     -- 3. Run Dijkstra
                                     RETURN QUERY
                                     SELECT 
                                         a.seq,
                                         ST_AsGeoJSON(b.way)::text as geo_json,
                                         ST_Length(b.way::geography)::float as distance_meters
                                     FROM pgr_dijkstra(
                                         'SELECT osm_id as id, source, target, st_length(way) as cost FROM planet_osm_line',
                                         start_node_id,
                                         end_node_id,
                                         false
                                     ) as a
                                     JOIN planet_osm_line as b ON a.edge = b.osm_id
                                     ORDER BY a.seq;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
