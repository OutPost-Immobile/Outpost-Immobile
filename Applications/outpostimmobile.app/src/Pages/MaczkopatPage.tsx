import {Button, Stack, Paper, TextField, Typography} from "@mui/material";
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
        {field: "id", headerName: "#", width: 70},
        {field: "friendlyId", headerName: "ID", width: 215},
        {field: "status", headerName: "Status", width: 215}
    ];

    return (
        <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 16}}>
            <Paper elevation={6} style={{padding: 16, margin: 16}}>
                <Stack spacing={5} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 16}}>
                    <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                        Zawartość maczkopatu
                    </Typography>
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
                        onClick={handleButtonClick}
                        variant="contained"
                        sx={{width: 500, height: 64, color: '#323232', backgroundColor: '#FFDE59'}}>
                        Sprawdź
                    </Button>
                </Stack>
            </Paper>
            <Paper elevation={6} style={{padding: 16}}>
                <DataGrid
                    rows={parcels.map((p, index) => ({
                        id: index+1,
                        friendlyId: p.friendlyId,
                        status: p.status
                    }))}
                    columns={columns}
                    loading={loading}
                    initialState={{
                        pagination: {
                            paginationModel: {
                                pageSize: 5,
                            },
                        },
                    }}
                    pageSizeOptions={[5]}
                    sx={{
                        boxShadow: 2,
                        border: 2,
                        borderColor: '#FFDE59',
                        "& .MuiDataGrid-footerContainer": {
                            backgroundColor: "#FFDE59",
                            color: "black",
                        }
                    }}
                >
                </DataGrid>
            </Paper>
        </Stack>
    )
}