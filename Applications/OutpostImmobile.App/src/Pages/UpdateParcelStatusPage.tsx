import { Alert, Button, MenuItem, Paper, Stack, TextField, Typography } from "@mui/material";
import { useMemo, useState } from "react";
import { $api } from "../Api/Api.ts";
import { PARCEL_UPDATE_URL, POST_METHOD } from "../Consts.ts";

export const UpdateParcelStatusPage = () => {
    const [loading, setLoading] = useState(false);

    const [parcelId, setParcelId] = useState("");
    const [parcelStatus, setParcelStatus] = useState<number | "">("");

    const [error, setError] = useState<string>("");
    const [success, setSuccess] = useState<string>("");

    const statusOptions = useMemo(
        () =>
            [
                { label: "Expedited", value: 0 },
                { label: "Delivered", value: 1 },
                { label: "InTransit", value: 2 },
                { label: "InWarehouse", value: 3 },
                { label: "Forgotten", value: 4 },
                { label: "Deleted", value: 5 },
                { label: "Sent", value: 6 },
                { label: "ToReturn", value: 7 },
                { label: "SendToStorage", value: 8 },
                { label: "InMaczkopat", value: 9 },
            ] as const,
        []
    );

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
        } catch {
            setError("Nie udało się zaktualizować statusu paczki.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Stack spacing={2} sx={{ justifyContent: "center", alignItems: "center", paddingTop: 8 }}>
            <Paper elevation={6} sx={{ padding: 2, margin: 8, width: 600, maxWidth: "95vw" }}>
                <Stack spacing={3}>
                    <Typography variant="h5" sx={{ fontWeight: "bold", textAlign: "center" }}>
                        Aktualizacja statusu paczki
                    </Typography>

                    <TextField
                        required
                        id="parcelId-field"
                        label="Friendly-Id paczki"
                        variant="outlined"
                        value={parcelId}
                        onChange={(e) => setParcelId(e.target.value)}
                        sx={{ width: "100%" }}
                    />

                    <TextField
                        required
                        select
                        id="status-field"
                        label="Status paczki"
                        variant="outlined"
                        value={parcelStatus}
                        onChange={(e) => {
                            const raw = e.target.value;
                            setParcelStatus(raw === "" ? "" : Number(raw));
                        }}
                        sx={{ width: "100%" }}
                    >
                        {statusOptions.map((s) => (
                            <MenuItem key={s.value} value={s.value}>
                                {s.label}
                            </MenuItem>
                        ))}
                    </TextField>

                    {error && <Alert severity="error">{error}</Alert>}
                    {success && <Alert severity="success">{success}</Alert>}

                    <Button
                        onClick={handleButtonClick}
                        disabled={loading}
                        variant="contained"
                        sx={{ width: "100%", height: 56, color: "#323232", backgroundColor: "#FFDE59" }}
                    >
                        {loading ? "Aktualizowanie..." : "Zaktualizuj status"}
                    </Button>
                </Stack>
            </Paper>
        </Stack>
    );
};