import { Button, Stack, Paper, TextField, Typography, Box, Divider, Card, CardContent, CardActions } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useState } from "react";
import { useNavigate } from "react-router";
import { $api } from "../Api/Api.ts";
import { GET_METHOD, MACZKOPAT_PARCELS_URL } from "../Consts.ts";
import ListAltIcon from "@mui/icons-material/ListAlt";
import SearchIcon from "@mui/icons-material/Search";

export const MaczkopatPage = () => {
    const navigate = useNavigate();
    const [maczkopatId, setMaczkopatId] = useState("");
    const [searchId, setSearchId] = useState<string | null>(null);

    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        MACZKOPAT_PARCELS_URL,
        {
            params: {
                path: { maczkopatId: searchId as string },
            },
        },
        {
            enabled: !!searchId,
        }
    );

    const handleButtonClick = () => {
        if (maczkopatId.trim()) {
            setSearchId(maczkopatId);
        }
    }

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') {
            handleButtonClick();
        }
    }

    const parcels = data?.data ?? [];

    const columns: GridColDef[] = [
        { field: "id", headerName: "#", width: 50 },
        { field: "friendlyId", headerName: "ID Paczki", flex: 1, minWidth: 150 },
        { field: "status", headerName: "Status", flex: 1, minWidth: 100 }
    ];

    return (
        <Box sx={{ p: { xs: 1, sm: 2, md: 3 }, height: '100%', overflow: 'auto' }}>
            <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold', mb: 2 }}>
                Zarządzanie Maczkopatami
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
                <Card sx={{ minWidth: 200, flex: 1, maxWidth: { xs: '100%', md: 320 } }}>
                    <CardContent sx={{ p: { xs: 1.5, sm: 2 }, '&:last-child': { pb: { xs: 1.5, sm: 2 } } }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <ListAltIcon sx={{ fontSize: 32, color: '#FFDE59', mr: 1.5 }} />
                            <Typography variant="subtitle1" fontWeight="bold">Lista Maczkopatów</Typography>
                        </Box>
                        <Typography variant="body2" color="text.secondary" sx={{ fontSize: '0.85rem' }}>
                            Przeglądaj wszystkie maczkopaty w systemie.
                        </Typography>
                    </CardContent>
                    <CardActions sx={{ pt: 0 }}>
                        <Button 
                            size="small" 
                            onClick={() => navigate('/Maczkopats')}
                            sx={{ color: '#323232', fontWeight: 'bold' }}
                        >
                            Przejdź do listy →
                        </Button>
                    </CardActions>
                </Card>
                <Card sx={{ minWidth: 200, flex: 1, maxWidth: { xs: '100%', md: 320 } }}>
                    <CardContent sx={{ p: { xs: 1.5, sm: 2 }, '&:last-child': { pb: { xs: 1.5, sm: 2 } } }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <SearchIcon sx={{ fontSize: 32, color: '#FFDE59', mr: 1.5 }} />
                            <Typography variant="subtitle1" fontWeight="bold">Szybkie wyszukiwanie</Typography>
                        </Box>
                        <Typography variant="body2" color="text.secondary" sx={{ fontSize: '0.85rem' }}>
                            Wpisz ID poniżej, aby sprawdzić zawartość.
                        </Typography>
                    </CardContent>
                </Card>
            </Box>

            <Divider sx={{ my: 2 }} />
            <Paper elevation={3} sx={{ p: 2, mb: 2 }}>
                <Typography variant="subtitle1" sx={{ fontWeight: 'bold', mb: 1.5 }}>
                    Sprawdź zawartość maczkopatu
                </Typography>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} alignItems="stretch">
                    <TextField
                        required
                        size="small"
                        id="id-field"
                        label="ID Maczkopatu"
                        variant="outlined"
                        value={maczkopatId}
                        onChange={(e) => setMaczkopatId(e.target.value)}
                        onKeyPress={handleKeyPress}
                        error={isError}
                        helperText={isError ? "Nie znaleziono" : ""}
                        sx={{ flex: 1 }}
                    />
                    <Button
                        onClick={handleButtonClick}
                        variant="contained"
                        startIcon={<SearchIcon />}
                        sx={{ 
                            height: 40, 
                            px: 3,
                            color: '#323232', 
                            backgroundColor: '#FFDE59',
                            '&:hover': {
                                backgroundColor: '#E5C84F',
                            }
                        }}
                    >
                        Sprawdź
                    </Button>
                </Stack>

                {searchId && maczkopatId && (
                    <Button
                        variant="outlined"
                        size="small"
                        onClick={() => navigate(`/Maczkopat/${searchId}`)}
                        sx={{ mt: 1.5 }}
                    >
                        Zobacz szczegóły →
                    </Button>
                )}
            </Paper>
            {searchId && (
                <Paper elevation={3} sx={{ p: 2, flex: 1 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                        <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                            Zawartość: {searchId}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            {parcels.length} paczek
                        </Typography>
                    </Box>
                    <DataGrid
                        rows={parcels.map((p, index) => ({
                            id: index + 1,
                            friendlyId: p.friendlyId,
                            status: p.status ?? '-'
                        }))}
                        columns={columns}
                        loading={isLoading}
                        density="compact"
                        initialState={{
                            pagination: {
                                paginationModel: { pageSize: 10 },
                            },
                        }}
                        pageSizeOptions={[10, 25]}
                        disableRowSelectionOnClick
                        sx={{
                            height: 350,
                            boxShadow: 1,
                            border: 1,
                            borderColor: '#FFDE59',
                            "& .MuiDataGrid-footerContainer": {
                                backgroundColor: "#FFDE59",
                                color: "black",
                            }
                        }}
                    />
                </Paper>
            )}
        </Box>
    )
}