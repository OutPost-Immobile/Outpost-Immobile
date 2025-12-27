import {Button, Paper, Stack, TextField, Typography} from "@mui/material";
import {useState} from "react";

export const LoginPage = () => {
    const [loading, setLoading] = useState(false);
    function handleButtonClick() {
        setLoading(true);
    }
    return (
        <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 64}}>
            <Paper elevation={6} style={{padding: 16, margin: 64}}>
                <Stack spacing={5}>
                    <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                        Logowanie
                    </Typography>
                    <TextField
                    required
                    id="outlined-basic"
                    label="Email"
                    variant="outlined"
                    sx={{width: 500, height: 32}}/>
                    <TextField
                        required
                        type="password"
                        id="outlined-basic"
                        label="Hasło"
                        variant="outlined"
                        sx={{width: 500, height: 32}}/>
                    <Button
                        onClick={handleButtonClick}
                        loading={loading}
                        variant="contained"
                        sx={{width: 500, height: 64, color: '#323232', backgroundColor: '#FFDE59'}}>
                        Zaloguj
                    </Button>
                </Stack>
            </Paper>
        </Stack>
    )
}