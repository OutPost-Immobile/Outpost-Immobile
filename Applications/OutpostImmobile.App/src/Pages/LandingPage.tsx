import {Button, Grid2, Paper, Stack, Typography} from "@mui/material";
import LOGO  from "../assets/image2vector.svg";
import {useAuth} from "../Auth/AuthProvider.tsx";
import {useState} from "react";
import {useNavigate} from "react-router"; // Add this import

export const LandingPage = () => {
    const { isAuthenticated } = useAuth();
    const [loading] = useState(false);
    const navigate = useNavigate(); // Add this hook

    const handleMaczkopatState = () => {
        navigate('/Maczkopat');
    }

    const handleRouteListPath = () => {
        navigate('/Routes');
    }

    const handleParcelStateUpdatePath = () => {
        navigate('/Parcels/UpdateStatus');
    }

    return (
        <Grid2 container spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 64}}>
            <Grid2 size={12}>
                <Typography variant="h2" style={{fontWeight: 'bold', textAlign: 'center'}}>
                    Witaj w serwisie Outpost Immobile
                </Typography>
            </Grid2>
            <Grid2 size={12}>
                <Typography variant="h3" style={{fontWeight: 'bold', textAlign: 'center'}}>
                    Zaufaj nam, a my zrobimy resztę
                </Typography>
            </Grid2>

            {!isAuthenticated ? (
                <>
                    <Grid2 size={4}>
                        <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                            <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                                Nadaj Paczkę
                            </Typography>
                            <img src={LOGO} alt="Outpost"
                                 style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                        </Paper>
                    </Grid2>
                    <Grid2 size={4}>
                        <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                            <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                                Zaskocz nas
                            </Typography>
                            <img src={LOGO} alt="Outpost"
                                 style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                        </Paper>
                    </Grid2>
                    <Grid2 size={4}>
                        <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                            <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                                Śledź Paczkę
                            </Typography>
                            <img src={LOGO} alt="Outpost"
                                 style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                        </Paper>
                    </Grid2>
                </>
            ) : (
                <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 64}}>
                    <Paper elevation={6} style={{padding: 16}}>
                        <Stack spacing={5}>
                            <Button
                                onClick={handleMaczkopatState}
                                disabled={loading}
                                variant="contained"
                                sx={{width: 500, height: 64, color: '#323232', backgroundColor: '#FFDE59'}}>
                                Stan maczkopatu
                            </Button>
                            <Button
                                onClick={handleRouteListPath}
                                disabled={loading}
                                variant="contained"
                                sx={{width: 500, height: 64, color: '#323232', backgroundColor: '#FFDE59'}}>
                                Lista tras
                            </Button>
                            <Button
                                onClick={handleParcelStateUpdatePath}
                                disabled={loading}
                                variant="contained"
                                sx={{width: 500, height: 64, color: '#323232', backgroundColor: '#FFDE59'}}>
                                Aktualizacja stanu paczki
                            </Button>
                        </Stack>
                    </Paper>
                </Stack>
            )}

            <Grid2 size={12}>
                <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center', paddingTop: 16}}>
                    Zaufało nam już dużo ludzi!
                </Typography>
            </Grid2>
        </Grid2>
    )
}