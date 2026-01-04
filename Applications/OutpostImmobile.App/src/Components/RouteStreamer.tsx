import React, { useEffect, useRef } from 'react';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import { useRouteQuery } from '../Hooks/useRouteQuery';

interface RouteStreamerProps {
    routeId: number;
}

export const RouteStreamer: React.FC<RouteStreamerProps> = ({ routeId }) => {
    const map = useMap();
    const layerRef = useRef<L.GeoJSON | null>(null);

    useEffect(() => {
        if (!layerRef.current) {
            layerRef.current = L.geoJSON(null, {
                style: { color: '#e63946', weight: 5, opacity: 0.8 }
            }).addTo(map);
        }
        return () => {
            layerRef.current?.remove();
            layerRef.current = null;
        };
    }, [map]);

    const { data, isLoading, isError } = useRouteQuery(routeId, layerRef);
    
    useEffect(() => {
        if (data && layerRef.current?.getBounds().isValid()) {
            map.fitBounds(layerRef.current.getBounds(), { padding: [50, 50] });
        }
    }, [data, map]);

    return (
        <div className="leaflet-top leaflet-right">
            <div className="leaflet-control leaflet-bar" style={{
                background: 'white', padding: '10px', marginTop: '10px', marginRight: '10px'
            }}>
                {isLoading && <div>Loading Route #{routeId}...</div>}
                {isError && <div style={{color:'red'}}>Failed to load</div>}
                {data && <div><strong>{(data.totalDistance / 1000).toFixed(2)} km</strong></div>}
            </div>
        </div>
    );
};