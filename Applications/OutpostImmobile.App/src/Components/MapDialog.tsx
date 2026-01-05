import React from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box
} from "@mui/material";
import { MapContainer, TileLayer } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import { RouteStreamer } from './RouteStreamer';

interface RouteMapDialogProps {
    open: boolean;
    onClose: () => void;
    route: {
        routeId: number | string;
        startAddressName: string;
        endAddressName: string;
    } | null;
}

export const RouteMapDialog: React.FC<RouteMapDialogProps> = ({ open, onClose, route }) => {
    if (!route) return null;

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="lg"
            fullWidth
        >
            <DialogTitle>
                Widok trasy: Z {route.startAddressName} do {route.endAddressName}
            </DialogTitle>
            <DialogContent dividers sx={{ p: 0, height: '600px' }}>
                <Box sx={{ height: '100%', width: '100%' }}>
                    <MapContainer
                        center={[52.2297, 21.0122]}
                        zoom={13}
                        style={{ height: '100%', width: '100%' }}
                    >
                        <TileLayer
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        />
                        <RouteStreamer routeId={Number(route.routeId)} />
                    </MapContainer>
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} color="primary">Close</Button>
            </DialogActions>
        </Dialog>
    );
};