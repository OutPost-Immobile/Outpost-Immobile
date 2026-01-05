import { useQuery } from '@tanstack/react-query';
import { useEffect, useRef } from 'react';
import L from 'leaflet';
import { fetchRouteStream } from "../Api/RouteApi"; // Check path

export function useRouteQuery(
    routeId: number | string | undefined,
    mapLayerRef: React.RefObject<L.GeoJSON | null>,
    isLayerReady: boolean,
    map: L.Map // Pass the map instance to handle zooming
) {
    const idAsNumber = typeof routeId === 'string' ? parseInt(routeId, 10) : routeId;
    const hasFitBounds = useRef(false); // Track if we have zoomed yet

    const query = useQuery({
        queryKey: ['routeId', idAsNumber],
        queryFn: async () => {
            if (!idAsNumber) throw new Error("Route ID is required");

            hasFitBounds.current = false; // Reset zoom tracker for new fetch

            if (mapLayerRef.current) {
                mapLayerRef.current.clearLayers();
            }

            return fetchRouteStream(idAsNumber, (segment) => {
                if (!mapLayerRef.current) return;

                if (segment.geoJson) {
                    try {
                        const geometry = JSON.parse(segment.geoJson);

                        mapLayerRef.current.addData(geometry);

                        if (!hasFitBounds.current && mapLayerRef.current.getLayers().length > 0) {
                            const bounds = mapLayerRef.current.getBounds();
                            if (bounds.isValid()) {
                                map.fitBounds(bounds, { padding: [50, 50] });
                                hasFitBounds.current = true;
                            }
                        }
                    } catch (e) {
                        console.error("Error parsing GeoJSON segment:", e);
                    }
                }
            });
        },
        enabled: !!idAsNumber && isLayerReady,
        refetchOnWindowFocus: false,
        staleTime: 1000 * 60 * 10, // Cache for 10 mins
    });

    useEffect(() => {
        const layer = mapLayerRef.current;

        if (query.data?.segments && layer && isLayerReady && layer.getLayers().length === 0) {

            query.data.segments.forEach(segment => {
                if (segment.geoJson) {
                    try {
                        layer.addData(JSON.parse(segment.geoJson));
                    } catch (e) { /* ignore */ }
                }
            });

            const bounds = layer.getBounds();
            if (bounds.isValid()) {
                map.fitBounds(bounds, { padding: [50, 50] });
            }
        }
    }, [query.data, mapLayerRef, isLayerReady, map]);

    return query;
}