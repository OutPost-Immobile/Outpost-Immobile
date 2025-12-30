import {Button, Paper, Stack, TextField, Typography} from "@mui/material";
import {useState} from "react";

export const LoginPage = () => {
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleButtonClick = async () => {
        setLoading(true);
        setError("");

        try {
            const response = await fetch("/login", {
                method: "POST",
                body: JSON.stringify({
                    email: email,
                    password: password,
                    rememberMe: false,
                })
            });

            if (response.ok) {
                window.location.href = '';
            } else {
                setError("Błąd logowania");
            }
        } catch (err) {
            setError("Błąd logowania");
        } finally {
            setLoading(false);
        }
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
                        id="email-field"
                        label="Email"
                        variant="outlined"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        error={!!error}
                        sx={{width: 500, height: 32}}/>
                    <TextField
                        required
                        type="password"
                        id="password-field"
                        label="Hasło"
                        variant="outlined"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        error={!!error}
                        sx={{width: 500, height: 32}}/>
                    {error && (
                        <Typography color="error" sx={{textAlign: 'center'}}>
                            {error}
                        </Typography>
                    )}
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