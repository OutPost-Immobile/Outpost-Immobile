import { Button, Stack, Paper, TextField } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import type { ParcelDto, TypedResponse } from "../Models/Types.ts";
import { useState } from "react";

export const MaczkopatPage = () => {

    const [maczkopatId, setMaczkopatId] = useState("");
    const [parcels, setParcels] = useState<ParcelDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const handleButtonClick = async () => {
        setLoading(true);
        setError("");

        try {
            const response = await fetch(`http://localhost:5188/api/Parcels/Maczkopat/${maczkopatId}`);
            const result: TypedResponse<ParcelDto[]> = await response.json();

            if (result.data) {
                setParcels(result.data);
            } else if (result.errors) {
                console.error(result.errors);
            }
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    }

    const columns: GridColDef[] = [
        {field: "friendlyId", headerName: "Friendly ID", width: 300},
        {field: "status", headerName: "Parcel status", width: 300}
    ];

    return (
        <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 64}}>
            <Paper elevation={6} style={{padding: 16, margin: 16}}>
                <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 16}}>
                    <TextField
                        required
                        id="id-field"
                        label="ID Maczkopatu"
                        variant="outlined"
                        value={maczkopatId}
                        onChange={(e) => setMaczkopatId(e.target.value)}
                        error={!!error}
                        sx={{width: 500}}
                    />
                    <Button
                        variant="outlined"
                        onClick={handleButtonClick}>
                    </Button>
                </Stack>
            </Paper>
            <Paper elevation={6} style={{padding: 16}}>
                <DataGrid
                    rows={parcels.map((p, index) => ({
                        id: index,
                        friendlyId: p.friendlyId,
                        status: p.status
                    }))}
                    columns={columns}
                    autoHeight
                    loading={loading}
                    pageSizeOptions={[5, 10, 25]}
                ></DataGrid>
            </Paper>
        </Stack>
    )
}