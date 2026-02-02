import { Box, Button, Card, CardActions, CardContent, Typography } from "@mui/material";
import LOGO from "../assets/image2vector.svg";
import { useAuth } from "../Auth/AuthProvider.tsx";
import { useNavigate } from "react-router";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import RouteIcon from "@mui/icons-material/Route";
import UpdateIcon from "@mui/icons-material/Update";
import InventoryIcon from "@mui/icons-material/Inventory";

export const LandingPage = () => {
    const { isAuthenticated } = useAuth();
    const navigate = useNavigate();

    const handleMaczkopatState = () => navigate('/Maczkopat');
    const handleRouteListPath = () => navigate('/Routes');
    const handleParcelStateUpdatePath = () => navigate('/Parcels/UpdateStatus');

    return (
        <Box sx={{ 
            display: 'flex', 
            flexDirection: 'column', 
            alignItems: 'center', 
            p: { xs: 2, sm: 3, md: 4 },
            minHeight: '100%'
        }}>
            <Typography 
                variant="h4" 
                sx={{ 
                    fontWeight: 'bold', 
                    textAlign: 'center',
                    mb: 1
                }}
            >
                Witaj w serwisie Outpost Immobile
            </Typography>
            <Typography 
                variant="h6" 
                color="text.secondary"
                sx={{ textAlign: 'center', mb: 3 }}
            >
                Zaufaj nam, a my zrobimy resztę
            </Typography>

            {!isAuthenticated ? (
                <Box sx={{ 
                    display: 'flex', 
                    gap: 2, 
                    flexWrap: 'wrap', 
                    justifyContent: 'center',
                    mb: 3,
                    maxWidth: 900
                }}>
                    <Card sx={{ width: { xs: '100%', sm: 260 } }}>
                        <CardContent sx={{ textAlign: 'center', p: 2 }}>
                            <InventoryIcon sx={{ fontSize: 48, color: '#FFDE59', mb: 1 }} />
                            <Typography variant="h6" fontWeight="bold">Nadaj Paczkę</Typography>
                            <img src={LOGO} alt="Outpost" style={{ width: 120, height: 120, marginTop: 8 }} />
                        </CardContent>
                    </Card>
                    <Card sx={{ width: { xs: '100%', sm: 260 } }}>
                        <CardContent sx={{ textAlign: 'center', p: 2 }}>
                            <LocalShippingIcon sx={{ fontSize: 48, color: '#FFDE59', mb: 1 }} />
                            <Typography variant="h6" fontWeight="bold">Zaskocz nas</Typography>
                            <img src={LOGO} alt="Outpost" style={{ width: 120, height: 120, marginTop: 8 }} />
                        </CardContent>
                    </Card>
                    <Card sx={{ width: { xs: '100%', sm: 260 } }}>
                        <CardContent sx={{ textAlign: 'center', p: 2 }}>
                            <RouteIcon sx={{ fontSize: 48, color: '#FFDE59', mb: 1 }} />
                            <Typography variant="h6" fontWeight="bold">Śledź Paczkę</Typography>
                            <img src={LOGO} alt="Outpost" style={{ width: 120, height: 120, marginTop: 8 }} />
                        </CardContent>
                    </Card>
                </Box>
            ) : (
                <Box sx={{ 
                    display: 'flex', 
                    gap: 2, 
                    flexWrap: 'wrap', 
                    justifyContent: 'center',
                    mb: 3,
                    maxWidth: 900
                }}>
                    <Card sx={{ width: { xs: '100%', sm: 280 } }}>
                        <CardContent sx={{ p: 2 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <LocalShippingIcon sx={{ fontSize: 36, color: '#FFDE59', mr: 1.5 }} />
                                <Typography variant="h6" fontWeight="bold">Maczkopaty</Typography>
                            </Box>
                            <Typography variant="body2" color="text.secondary">
                                Sprawdź stan i zawartość maczkopatów w systemie.
                            </Typography>
                        </CardContent>
                        <CardActions sx={{ pt: 0 }}>
                            <Button 
                                size="small" 
                                onClick={handleMaczkopatState}
                                sx={{ color: '#323232', fontWeight: 'bold' }}
                            >
                                Przejdź →
                            </Button>
                        </CardActions>
                    </Card>
                    
                    <Card sx={{ width: { xs: '100%', sm: 280 } }}>
                        <CardContent sx={{ p: 2 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <RouteIcon sx={{ fontSize: 36, color: '#FFDE59', mr: 1.5 }} />
                                <Typography variant="h6" fontWeight="bold">Trasy</Typography>
                            </Box>
                            <Typography variant="body2" color="text.secondary">
                                Przeglądaj i zarządzaj trasami dostaw.
                            </Typography>
                        </CardContent>
                        <CardActions sx={{ pt: 0 }}>
                            <Button 
                                size="small" 
                                onClick={handleRouteListPath}
                                sx={{ color: '#323232', fontWeight: 'bold' }}
                            >
                                Przejdź →
                            </Button>
                        </CardActions>
                    </Card>
                    
                    <Card sx={{ width: { xs: '100%', sm: 280 } }}>
                        <CardContent sx={{ p: 2 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <UpdateIcon sx={{ fontSize: 36, color: '#FFDE59', mr: 1.5 }} />
                                <Typography variant="h6" fontWeight="bold">Status paczki</Typography>
                            </Box>
                            <Typography variant="body2" color="text.secondary">
                                Aktualizuj status pojedynczych paczek.
                            </Typography>
                        </CardContent>
                        <CardActions sx={{ pt: 0 }}>
                            <Button 
                                size="small" 
                                onClick={handleParcelStateUpdatePath}
                                sx={{ color: '#323232', fontWeight: 'bold' }}
                            >
                                Przejdź →
                            </Button>
                        </CardActions>
                    </Card>
                </Box>
            )}

            <Typography 
                variant="body1" 
                sx={{ fontWeight: 'bold', textAlign: 'center', color: 'text.secondary' }}
            >
                Zaufało nam już dużo ludzi!
            </Typography>
        </Box>
    );
}