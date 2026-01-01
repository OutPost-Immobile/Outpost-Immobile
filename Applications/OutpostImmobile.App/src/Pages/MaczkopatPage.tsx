import {Button, Stack, Paper, TextField, Typography} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useState } from "react";
import {$api} from "../Api/Api.ts";
import {GET_METHOD, MACZKOPAT_PARCELS_URL} from "../Consts.ts";

export const MaczkopatPage = () => {
    const [maczkopatId, setMaczkopatId] = useState("");
    const [searchId, setSearchId] = useState<string | null>(null);

    // useQuery will automatically trigger when searchId changes
    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        MACZKOPAT_PARCELS_URL,
        {
            params: {
                path: { maczkopatId: searchId as string },
            },
        },
        {
            enabled: !!searchId, // Only run the query if we have an ID to search for
        }
    );

    const handleButtonClick = () => {
        if (maczkopatId.trim()) {
            setSearchId(maczkopatId);
        }
    }

    // Extract parcels safely from the typed response
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
                        Zawartość maczkopatu
                    </Typography>
                    <TextField
                        required
                        id="id-field"
                        label="ID Maczkopatu"
                        variant="outlined"
                        value={maczkopatId}
                        onChange={(e) => setMaczkopatId(e.target.value)}
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