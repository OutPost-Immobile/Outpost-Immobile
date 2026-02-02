import { Alert, Button, Paper, Stack, TextField, Typography, Box } from "@mui/material";
import { useState } from "react";
import { $api } from "../Api/Api.ts";
import { PARCEL_UPDATE_URL, POST_METHOD } from "../Consts.ts";
import UpdateIcon from "@mui/icons-material/Update";
import { StatusDropdown } from "../Components/StatusDropdown.tsx";

export const UpdateParcelStatusPage = () => {
    const [loading, setLoading] = useState(false);

    const [parcelId, setParcelId] = useState("");
    const [parcelStatus, setParcelStatus] = useState<number | "">("");

    const [error, setError] = useState<string>("");
    const [success, setSuccess] = useState<string>("");

    const { mutateAsync } = $api.useMutation(POST_METHOD, PARCEL_UPDATE_URL);

    const handleButtonClick = async () => {
        setLoading(true);
        setError("");
        setSuccess("");

        const friendlyId = parcelId.trim();

        if (!friendlyId) {
            setError("Podaj Friendly-Id paczki.");
            setLoading(false);
            return;
        }

        if (parcelStatus === "") {
            setError("Wybierz status paczki.");
            setLoading(false);
            return;
        }

        try {
            await mutateAsync({
                body: [
                    {
                        friendlyId,
                        parcelStatus,
                    },
                ],
            });

            setSuccess("Status paczki został zaktualizowany.");
            setParcelId("");
            setParcelStatus("");
        } catch {
            setError("Nie udało się zaktualizować statusu paczki.");
        } finally {
            setLoading(false);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && parcelId.trim() && parcelStatus !== "") {
            handleButtonClick();
        }
    };

    return (
        <Box sx={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            minHeight: 'calc(100vh - 100px)',
            p: 2
        }}>
            <Paper elevation={6} sx={{ 
                p: { xs: 2, sm: 4 }, 
                width: '100%', 
                maxWidth: 500 
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
                        <UpdateIcon sx={{ fontSize: 32, color: '#323232' }} />
                    </Box>
                    <Typography variant="h5" sx={{ fontWeight: "bold", textAlign: "center" }}>
                        Aktualizacja statusu paczki
                    </Typography>

                    <TextField
                        required
                        fullWidth
                        id="parcelId-field"
                        label="Friendly-Id paczki"
                        variant="outlined"
                        value={parcelId}
                        onChange={(e) => setParcelId(e.target.value)}
                        onKeyPress={handleKeyPress}
                    />

                    <StatusDropdown
                        value={parcelStatus}
                        onChange={setParcelStatus}
                        label="Status paczki"
                        fullWidth
                    />

                    {error && <Alert severity="error" sx={{ width: '100%' }}>{error}</Alert>}
                    {success && <Alert severity="success" sx={{ width: '100%' }}>{success}</Alert>}

                    <Button
                        fullWidth
                        onClick={handleButtonClick}
                        disabled={loading}
                        variant="contained"
                        size="large"
                        sx={{ 
                            height: 48, 
                            color: "#323232", 
                            backgroundColor: "#FFDE59",
                            '&:hover': {
                                backgroundColor: '#E5C84F',
                            }
                        }}
                    >
                        {loading ? "Aktualizowanie..." : "Zaktualizuj status"}
                    </Button>
                </Stack>
            </Paper>
        </Box>
    );
};