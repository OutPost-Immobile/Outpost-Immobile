import {
    Button,
    Stack,
    Paper,
    TextField,
    Typography,
    Box,
    CircularProgress,
    Alert,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Divider,
    Chip
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import { useState } from "react";
import { $api } from "../Api/Api.ts";
import { GET_METHOD, PARCEL_TRACK_URL } from "../Consts.ts";

export const TrackingPage = () => {
    const [parcelId, setParcelId] = useState("");
    const [searchId, setSearchId] = useState<string | null>(null);

    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        PARCEL_TRACK_URL,
        {
            params: {
                path: { parcelFriendlyId: searchId as string },
            },
        },
        {
            enabled: !!searchId,
        }
    );

    const handleButtonClick = () => {
        if (parcelId.trim()) {
            setSearchId(parcelId);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') {
            handleButtonClick();
        }
    };

    const logs = data?.data ?? [];

    const formatDate = (dateString: string | null | undefined) => {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleString('pl-PL', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 }, maxWidth: 800, mx: 'auto' }}>
            <Typography variant="h5" sx={{ fontWeight: 'bold', textAlign: 'center', mb: 3 }}>
                Śledzenie paczki
            </Typography>

            <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="stretch">
                    <TextField
                        required
                        fullWidth
                        id="parcel-id-field"
                        label="ID Paczki"
                        placeholder="Wpisz numer paczki..."
                        variant="outlined"
                        value={parcelId}
                        onChange={(e) => setParcelId(e.target.value)}
                        onKeyPress={handleKeyPress}
                        error={isError}
                        helperText={isError ? "Nie znaleziono paczki" : ""}
                    />
                    <Button
                        onClick={handleButtonClick}
                        variant="contained"
                        startIcon={<SearchIcon />}
                        sx={{
                            minWidth: 150,
                            height: 56,
                            color: '#323232',
                            backgroundColor: '#FFDE59',
                            '&:hover': {
                                backgroundColor: '#E5C84F',
                            }
                        }}
                    >
                        Śledź
                    </Button>
                </Stack>
            </Paper>

            {isLoading && (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                    <CircularProgress />
                </Box>
            )}

            {isError && searchId && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    Nie znaleziono paczki o podanym numerze.
                </Alert>
            )}

            {!isLoading && !isError && logs.length > 0 && (
                <Paper elevation={3} sx={{ p: 3 }}>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Historia paczki: {searchId}
                    </Typography>

                    <List>
                        {logs.map((log, index) => (
                            <Box key={index}>
                                <ListItem 
                                    sx={{ 
                                        py: 2,
                                        backgroundColor: index === 0 ? 'rgba(255, 222, 89, 0.1)' : 'transparent',
                                        borderRadius: 1
                                    }}
                                >
                                    <ListItemIcon>
                                        <LocalShippingIcon 
                                            sx={{ 
                                                color: index === 0 ? '#FFDE59' : 'text.secondary',
                                                fontSize: 28
                                            }} 
                                        />
                                    </ListItemIcon>
                                    <ListItemText
                                        primary={
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
                                                <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                                                    {log.parcelStatus ?? 'Status nieznany'}
                                                </Typography>
                                                {log.parcelEventLogType && (
                                                    <Chip 
                                                        label={log.parcelEventLogType} 
                                                        size="small" 
                                                        variant="outlined"
                                                    />
                                                )}
                                            </Box>
                                        }
                                        secondary={
                                            <Box>
                                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, color: 'text.secondary' }}>
                                                    <AccessTimeIcon sx={{ fontSize: 16 }} />
                                                    <Typography variant="body2">
                                                        {formatDate(log.createdAt)}
                                                    </Typography>
                                                </Box>
                                                {log.message && (
                                                    <Typography variant="body2" sx={{ mt: 0.5 }}>
                                                        {log.message}
                                                    </Typography>
                                                )}
                                            </Box>
                                        }
                                    />
                                </ListItem>
                                {index < logs.length - 1 && <Divider />}
                            </Box>
                        ))}
                    </List>
                </Paper>
            )}

            {!isLoading && !isError && searchId && logs.length === 0 && (
                <Alert severity="info">
                    Brak historii dla tej paczki.
                </Alert>
            )}
        </Box>
    );
};