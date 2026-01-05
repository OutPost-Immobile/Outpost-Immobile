import React, { useEffect, useRef, useState } from 'react';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import { useRouteQuery } from '../Hooks/useRouteQuery';

interface RouteStreamerProps {
    routeId: number;
}

export const RouteStreamer: React.FC<RouteStreamerProps> = ({ routeId }) => {
    const map = useMap();
    const layerRef = useRef<L.GeoJSON | null>(null);
    const [isLayerReady, setIsLayerReady] = useState(false);

    useEffect(() => {
        if (!layerRef.current) {
            layerRef.current = L.geoJSON(null, {
                style: { color: '#e63946', weight: 5, opacity: 0.8 }
            }).addTo(map);

            setIsLayerReady(true);
        }

        return () => {
            if (layerRef.current) {
                layerRef.current.remove();
                layerRef.current = null;
                setIsLayerReady(false);
            }
        };
    }, [map]);

    const { data, isLoading, isError } = useRouteQuery(routeId, layerRef, isLayerReady, map);

    return (
        <div className="leaflet-top leaflet-right">
            <div className="leaflet-control leaflet-bar" style={{
                background: 'white', padding: '12px', marginTop: '10px', marginRight: '10px',
                borderRadius: '4px', boxShadow: '0 1px 5px rgba(0,0,0,0.4)'
            }}>
                {isLoading && (
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <div className="spinner" style={{
                            width: '12px', height: '12px',
                            border: '2px solid #ccc', borderTopColor: '#333',
                            borderRadius: '50%', animation: 'spin 1s linear infinite'
                        }}/>
                        <span>Streaming Route...</span>
                    </div>
                )}

                {isError && <div style={{color:'red'}}>Failed to load route</div>}

                {data && (
                    <div>
                        <strong>Total: {(Number(data.totalDistance) / 1000).toFixed(2)} km</strong>
                    </div>
                )}

                <style>{`@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }`}</style>
            </div>
        </div>
    );
};