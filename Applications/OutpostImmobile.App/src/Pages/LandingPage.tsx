import {Grid2, Paper, Typography} from "@mui/material";
import LOGO  from "../assets/image2vector.svg";

export const LandingPage = () => {

    return (
        <>
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
                <Grid2 size={4}>
                    <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                        <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                            Nadaj Paczkę
                        </Typography>
                        <img src={LOGO} alt="Outpost" style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                    </Paper>
                </Grid2>
                <Grid2 size={4}>
                    <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                        <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                            Zaskocz nas
                        </Typography>
                        <img src={LOGO} alt="Outpost" style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                    </Paper>
                </Grid2>
                <Grid2 size={4}>
                    <Paper elevation={3} style={{padding: 16, marginInline: 16}}>
                        <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                            Śledź Paczkę
                        </Typography>
                        <img src={LOGO} alt="Outpost" style={{width: 256, height: 256, display: 'block', margin: 'auto'}}/>
                    </Paper>
                </Grid2>
                <Grid2 size={12}>
                    <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center', paddingTop: 16}}>
                        Zaufało nam już dużo ludzi!
                    </Typography>
                </Grid2>
            </Grid2>
        </>
    )
}