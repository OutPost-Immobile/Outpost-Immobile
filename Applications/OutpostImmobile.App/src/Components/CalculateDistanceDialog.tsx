import React from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    CircularProgress,
    Typography,
    Alert
} from "@mui/material";
import { $api } from "../Api/Api.ts";
import { GET_METHOD, ROUTE_CALCULATE_URL, ROUTE_SAVE_CALCULATION_URL } from "../Consts.ts";

interface CalculateDistanceDialogProps {
    open: boolean;
    onClose: () => void;
    routeId: number | null;
    onSaveSuccess?: () => void;
}

export const CalculateDistanceDialog: React.FC<CalculateDistanceDialogProps> = ({
    open,
    onClose,
    routeId,
    onSaveSuccess
}) => {
    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        ROUTE_CALCULATE_URL,
        {
            params: {
                path: { routeId: routeId as number },
            },
        },
        {
            enabled: !!routeId && open,
        }
    );

    const { mutateAsync, isPending: isSaving } = $api.useMutation(
        GET_METHOD,
        ROUTE_SAVE_CALCULATION_URL
    );

    const [saveError, setSaveError] = React.useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = React.useState(false);

    const distanceInMeters = data?.data ?? 0;
    const distanceInKm = (Number(distanceInMeters) / 1000).toFixed(2);

    const handleSave = async () => {
        if (!routeId || !distanceInMeters) return;
        
        setSaveError(null);
        setSaveSuccess(false);

        try {
            await mutateAsync({
                body: {
                    routeId: routeId,
                    calculatedDistance: Number(distanceInMeters)
                }
            });
            setSaveSuccess(true);
            onSaveSuccess?.();
        } catch {
            setSaveError("Nie udało się zapisać dystansu.");
        }
    };

    const handleClose = () => {
        setSaveError(null);
        setSaveSuccess(false);
        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            maxWidth="sm"
            fullWidth
        >
            <DialogTitle>
                Kalkulacja dystansu trasy
            </DialogTitle>
            <DialogContent dividers sx={{ minHeight: 150 }}>
                {isLoading && (
                    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 100 }}>
                        <CircularProgress />
                        <Typography sx={{ ml: 2 }}>Obliczanie dystansu...</Typography>
                    </Box>
                )}
                
                {isError && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        Wystąpił błąd podczas obliczania dystansu.
                    </Alert>
                )}
                
                {!isLoading && !isError && data && (
                    <Box sx={{ textAlign: 'center', py: 2 }}>
                        <Typography variant="h6" gutterBottom>
                            Obliczony dystans trasy:
                        </Typography>
                        <Typography variant="h3" color="primary" sx={{ fontWeight: 'bold', my: 2 }}>
                            {distanceInKm} km
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            ({Number(distanceInMeters).toLocaleString('pl-PL')} metrów)
                        </Typography>
                    </Box>
                )}

                {saveError && (
                    <Alert severity="error" sx={{ mt: 2 }}>
                        {saveError}
                    </Alert>
                )}

                {saveSuccess && (
                    <Alert severity="success" sx={{ mt: 2 }}>
                        Dystans został zapisany pomyślnie!
                    </Alert>
                )}
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} color="inherit">
                    Zamknij
                </Button>
                <Button
                    onClick={handleSave}
                    variant="contained"
                    disabled={isLoading || isError || isSaving || saveSuccess}
                    sx={{
                        backgroundColor: '#FFDE59',
                        color: '#323232',
                        '&:hover': { backgroundColor: '#E5C84F' }
                    }}
                >
                    {isSaving ? <CircularProgress size={20} /> : "Zapisz dystans"}
                </Button>
            </DialogActions>
        </Dialog>
    );
};
