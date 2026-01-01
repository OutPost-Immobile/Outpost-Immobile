import {Button, Paper, Stack, TextField, Typography} from "@mui/material";
import {useState} from "react";
import {$api} from "../Api/Api.ts";
import {LOGIN_URL, POST_METHOD} from "../Consts.ts";
import {useAuth} from "../Auth/AuthProvider.tsx";
import {useNavigate, useLocation} from "react-router";

export const LoginPage = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const { mutateAsync } = $api.useMutation(POST_METHOD, LOGIN_URL);

    const handleButtonClick = async () => {
        setLoading(true);
        setError("");

        try {
            const response = await mutateAsync({
                body: {
                    email: email,
                    password: password
                }
            });

            if (response) {
                // Update global auth state
                login(response);

                // Redirect to previous page or home
                const origin = location.state?.from?.pathname || "/";
                navigate(origin, { replace: true });
            }
        } catch (err) {
            setError("Błędny email lub hasło");
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