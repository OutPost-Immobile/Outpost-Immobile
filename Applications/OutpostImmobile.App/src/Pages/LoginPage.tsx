import {Button, Paper, Stack, TextField, Typography, Box} from "@mui/material";
import {useState} from "react";
import {$api} from "../Api/Api.ts";
import {LOGIN_URL, POST_METHOD} from "../Consts.ts";
import {useAuth} from "../Auth/AuthProvider.tsx";
import {useNavigate, useLocation} from "react-router";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";

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
                login(response);
                const origin = location.state?.from?.pathname || "/Maczkopat";
                navigate(origin, { replace: true });
            }
        } catch (err) {
            setError("Błędny email lub hasło");
        } finally {
            setLoading(false);
        }
    }

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') {
            handleButtonClick();
        }
    }

    return (
        <Box sx={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            minHeight: 'calc(100vh - 80px)',
            p: 2
        }}>
            <Paper elevation={6} sx={{ 
                p: { xs: 2, sm: 4 }, 
                width: '100%', 
                maxWidth: 450 
            }}>
                <Stack spacing={3} alignItems="center">
                    <Box sx={{ 
                        backgroundColor: '#FFDE59', 
                        borderRadius: '50%', 
                        p: 1.5,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center'
                    }}>
                        <LockOutlinedIcon sx={{ fontSize: 32, color: '#323232' }} />
                    </Box>
                    <Typography variant="h5" sx={{ fontWeight: 'bold', textAlign: 'center' }}>
                        Logowanie
                    </Typography>
                    <TextField
                        required
                        fullWidth
                        id="email-field"
                        label="Email"
                        variant="outlined"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        onKeyPress={handleKeyPress}
                        error={!!error}
                        size="medium"
                    />
                    <TextField
                        required
                        fullWidth
                        type="password"
                        id="password-field"
                        label="Hasło"
                        variant="outlined"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        onKeyPress={handleKeyPress}
                        error={!!error}
                        size="medium"
                    />
                    {error && (
                        <Typography color="error" sx={{ textAlign: 'center' }}>
                            {error}
                        </Typography>
                    )}
                    <Button
                        fullWidth
                        onClick={handleButtonClick}
                        loading={loading}
                        variant="contained"
                        size="large"
                        sx={{ 
                            height: 48, 
                            color: '#323232', 
                            backgroundColor: '#FFDE59',
                            '&:hover': {
                                backgroundColor: '#E5C84F',
                            }
                        }}
                    >
                        Zaloguj
                    </Button>
                </Stack>
            </Paper>
        </Box>
    )
}