import {Button, Stack, Paper, TextField, Typography} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useState } from "react";
import {$api} from "../Api/Api.ts";
import {GET_METHOD, PARCEL_LOGS_URL} from "../Consts.ts";

export const TrackingPage = () => {
    const [parcelId, setParcelId] = useState("");
    const [searchId, setSearchId] = useState<string | null>(null);

    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        PARCEL_LOGS_URL,
        {
            params: {
                path: { parcelId: searchId as string },
            },
        },
        {
            enabled: !!searchId,
        }
    );

    const handleButtonClick = () => {
        if (parcelId.trim()) {
            setSearchId(parcelId);
        }
    }

    const parcels = data?.data ?? [];

    const columns: GridColDef[] = [
        { field: "id", headerName: "#", width: 70 },
        { field: "friendlyId", headerName: "ID", width: 215 },
        { field: "status", headerName: "Status", width: 215 }
    ];

    return (
        <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 16}}>
            <Paper elevation={6} style={{padding: 16, margin: 16}}>
                <Stack spacing={5} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 16}}>
                    <Typography variant="h5" style={{fontWeight: 'bold', textAlign: 'center'}}>
                        Śledzenie paczki
                    </Typography>
                    <TextField
                        required
                        id="id-field"
                        label="ID Maczkopatu"
                        variant="outlined"
                        value={parcelId}
                        onChange={(e) => setParcelId(e.target.value)}
                        error={isError}
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
                        id: index + 1,
                        friendlyId: p.friendlyId,
                        status: p.status
                    }))}
                    columns={columns}
                    loading={isLoading}
                    initialState={{
                        pagination: {
                            paginationModel: { pageSize: 5 },
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
                />
            </Paper>
        </Stack>
    )
}