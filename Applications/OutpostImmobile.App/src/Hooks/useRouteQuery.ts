// useRouteQuery.ts
import {useQuery} from '@tanstack/react-query';
import {useEffect, useRef} from 'react';
import L from 'leaflet';
import {fetchRouteStream} from "../Api/RouteApi.ts";
import type {LatLng} from "../Types.ts";

export function useRouteQuery(
    start: LatLng,
    end: LatLng,
    mapLayerRef: React.MutableRefObject<L.GeoJSON | null>
) {
    const layerRef = useRef(mapLayerRef);
    layerRef.current = mapLayerRef;

    const query = useQuery({
        queryKey: ['route', start, end],
        queryFn: async () => {
            layerRef.current.current?.clearLayers();

            return fetchRouteStream(start, end, (segment) => {
                if (segment.geoJson && layerRef.current.current) {
                    const geometry = JSON.parse(segment.geoJson);
                    layerRef.current.current.addData(geometry);
                }
            });
        },
        refetchOnWindowFocus: false,
        staleTime: 1000 * 60 * 5,
    });
    
    useEffect(() => {
        if (query.data && layerRef.current.current) {
            const layer = layerRef.current.current;
            if (Object.keys(layer.getLayers()).length === 0) {
                console.log("Restoring route from cache...");
                query.data.segments.forEach(segment => {
                    if (segment.geoJson) {
                        layer.addData(JSON.parse(segment.geoJson));
                    }
                });
            }
        }
    }, [query.data]);

    return query;
}