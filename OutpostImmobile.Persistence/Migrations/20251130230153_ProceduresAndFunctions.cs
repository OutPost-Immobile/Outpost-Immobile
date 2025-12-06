using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// TODO 
namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProceduresAndFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .Annotation("Npgsql:PostgresExtension:pgrouting", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgrouting", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Parcels",
                type: "integer",
                nullable: true);

            // migrationBuilder.Sql("""
            //                      CREATE ROLE ADMIN;
            //                      CREATE ROLE APPUSER;
            //                      CREATE ROLE MANAGER;
            //                      CREATE ROLE COURIER;
            //                      GRANT CONNECT ON DATABASE "OutpostImmobile" TO ADMIN, APPUSER, MANAGER, COURIER;
            //                      GRANT USAGE ON SCHEMA public TO ADMIN, APPUSER, MANAGER, COURIER;
            //                      
            //                      -- Permissions for ADMIN
            //                      GRANT ALL PRIVILEGES
            //                          ON ALL TABLES
            //                          IN SCHEMA public
            //                          TO ADMIN;
            //                      
            //                      -- Permissions for USER
            //                      GRANT SELECT ON
            //                          "Parcels",
            //                          "ParcelEventLogs"
            //                          TO APPUSER;
            //                      
            //                      -- Permissions for MANAGER
            //                      GRANT SELECT ON
            //                          "ParcelEventLogs",
            //                          "CommunicationEventLogs",
            //                          "MaczkopatEventLogs",
            //                          "Routes", 
            //                          "Addresses",
            //                          "Maczkopats",
            //                          "Parcels",
            //                          "Vehicles",
            //                          "UsersInternal",
            //                          "UsersExternal"
            //                          TO MANAGER;
            //                      
            //                      GRANT UPDATE, DELETE ON
            //                          "Parcels",
            //                          "UsersInternal"
            //                          TO MANAGER;
            //                      
            //                      -- Permissions for COURIER
            //                      GRANT SELECT, UPDATE ON
            //                          "Routes",
            //                          "Vehicles",
            //                          "Parcels",
            //                          "Addresses"
            //                          TO COURIER;
            //                      """);

            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION get_vehicle_route_geojson(
                                     x1 DOUBLE PRECISION,
                                     y1 DOUBLE PRECISION,
                                     x2 DOUBLE PRECISION,
                                     y2 DOUBLE PRECISION
                                 )
                                     RETURNS JSONB AS $$
                                 DECLARE
                                     start_id BIGINT;
                                     end_id BIGINT;
                                     route_geom GEOMETRY;
                                     total_time_seconds DOUBLE PRECISION;
                                     total_dist_meters DOUBLE PRECISION;
                                     route_count INTEGER;
                                 BEGIN
                                     SELECT
                                         CASE
                                             WHEN ST_Distance(ST_StartPoint(the_geom), ST_SetSRID(ST_MakePoint(x1, y1), 4326)) <
                                                  ST_Distance(ST_EndPoint(the_geom), ST_SetSRID(ST_MakePoint(x1, y1), 4326))
                                                 THEN source
                                             ELSE target
                                             END
                                     INTO start_id
                                     FROM ways_main_network
                                     ORDER BY the_geom <-> ST_SetSRID(ST_MakePoint(x1, y1), 4326)
                                     LIMIT 1;
                                     SELECT
                                         CASE
                                             WHEN ST_Distance(ST_StartPoint(the_geom), ST_SetSRID(ST_MakePoint(x2, y2), 4326)) <
                                                  ST_Distance(ST_EndPoint(the_geom), ST_SetSRID(ST_MakePoint(x2, y2), 4326))
                                                 THEN source
                                             ELSE target
                                             END
                                     INTO end_id
                                     FROM ways_main_network
                                     ORDER BY the_geom <-> ST_SetSRID(ST_MakePoint(x2, y2), 4326)
                                     LIMIT 1;
                                 
                                     -- Check if nodes were found
                                     IF start_id IS NULL OR end_id IS NULL THEN
                                         RETURN jsonb_build_object(
                                                 'error', 'Could not find road network nodes in main network',
                                                 'start_id', start_id,
                                                 'end_id', end_id,
                                                 'hint', 'Coordinates may be too far from main road network'
                                                );
                                     END IF;
                                 
                                     -- Check if same node
                                     IF start_id = end_id THEN
                                         RETURN jsonb_build_object(
                                                 'error', 'Start and end are at the same location',
                                                 'node_id', start_id
                                                );
                                     END IF;
                                 
                                     -- Calculate Route (using ONLY main network roads)
                                     SELECT
                                         ST_LineMerge(ST_Collect(w.the_geom ORDER BY d.seq)),
                                         SUM(d.cost),
                                         SUM(ST_Length(w.the_geom::geography))
                                     INTO
                                         route_geom,
                                         total_time_seconds,
                                         total_dist_meters
                                     FROM pgr_dijkstra(
                                                  'SELECT id, source, target, cost_s AS cost, reverse_cost_s AS reverse_cost 
                                                   FROM ways_main_network',
                                                  start_id,
                                                  end_id,
                                                  directed := false  -- Allow both directions for now
                                          ) AS d
                                              JOIN ways AS w ON d.edge = w.id;
                                 
                                     -- Check if route was found
                                     IF route_geom IS NULL THEN
                                         RETURN jsonb_build_object(
                                                 'error', 'No route found between points',
                                                 'start_id', start_id,
                                                 'end_id', end_id
                                                );
                                     END IF;
                                 
                                     -- Return GeoJSON
                                     RETURN jsonb_build_object(
                                             'type', 'FeatureCollection',
                                             'features', jsonb_build_array(
                                                     jsonb_build_object(
                                                             'type', 'Feature',
                                                             'geometry', ST_AsGeoJSON(route_geom)::jsonb,
                                                             'properties', jsonb_build_object(
                                                                     'type', 'vehicle_route',
                                                                     'duration_minutes', ROUND((total_time_seconds / 60)::numeric, 2),
                                                                     'distance_km', ROUND((total_dist_meters / 1000)::numeric, 2),
                                                                     'distance_meters', ROUND(total_dist_meters::numeric, 2),
                                                                     'start_node', start_id,
                                                                     'end_node', end_id
                                                                           )
                                                     )
                                                         )
                                            );
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Parcels");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgrouting", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgrouting", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
