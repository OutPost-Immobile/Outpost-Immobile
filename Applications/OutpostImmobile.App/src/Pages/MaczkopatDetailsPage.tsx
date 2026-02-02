import { useState } from "react";
import { useParams, useNavigate } from "react-router";
import {
    Box,
    Typography,
    CircularProgress,
    Paper,
    Grid2 as Grid,
    Button,
    Stack,
    Chip,
    Alert
} from "@mui/material";
import {
    DataGrid,
    type GridColDef,
    type GridRowId,
    type GridRowSelectionModel
} from "@mui/x-data-grid";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { $api } from "../Api/Api.ts";
import { 
    GET_METHOD, 
    MACZKOPAT_BY_ID_URL, 
    MACZKOPAT_PARCELS_URL, 
    PARCEL_UPDATE_URL,
    POST_METHOD 
} from "../Consts.ts";
import { StatusDropdown } from "../Components/StatusDropdown.tsx";

interface MaczkopatDetails {
    id: string;
    code: string;
    capacity: number;
    areaName: string;
    city: string;
    postalCode: string;
    street: string;
    countryCode: string;
    buildingNumber: string;
    createdAt: string;
    updatedAt: string | null;
}

interface ParcelRow {
    id: string;
    friendlyId: string;
    status: string;
}

export const MaczkopatDetailsPage = () => {
    const { maczkopatId } = useParams<{ maczkopatId: string }>();
    const navigate = useNavigate();
    
    const { data: detailsData, isLoading: detailsLoading, isError: detailsError } = $api.useQuery(
        GET_METHOD,
        MACZKOPAT_BY_ID_URL,
        {
            params: {
                path: { maczkopatId: maczkopatId as string },
            },
        },
        {
            enabled: !!maczkopatId,
        }
    );
    
    const { data: parcelsData, isLoading: parcelsLoading, isError: parcelsError, refetch: refetchParcels } = $api.useQuery(
        GET_METHOD,
        MACZKOPAT_PARCELS_URL,
        {
            params: {
                path: { maczkopatId: maczkopatId as string },
            },
        },
        {
            enabled: !!maczkopatId,
        }
    );
    
    const { mutateAsync, isPending: isUpdating } = $api.useMutation(POST_METHOD, PARCEL_UPDATE_URL);

    const [selectedRows, setSelectedRows] = useState<GridRowId[]>([]);
    const [newStatus, setNewStatus] = useState<number | "">("");
    const [updateError, setUpdateError] = useState<string>("");
    const [updateSuccess, setUpdateSuccess] = useState<string>("");

    const handleSelectionChange = (newSelection: GridRowSelectionModel) => {
        setSelectedRows(Array.from(newSelection.ids));
        setUpdateError("");
        setUpdateSuccess("");
    };

    const handleMassUpdate = async () => {
        if (selectedRows.length === 0) {
            setUpdateError("Wybierz przynajmniej jedną paczkę.");
            return;
        }

        if (newStatus === "") {
            setUpdateError("Wybierz nowy status.");
            return;
        }

        setUpdateError("");
        setUpdateSuccess("");

        try {
            const updates = selectedRows.map((id) => ({
                friendlyId: id as string,
                parcelStatus: newStatus as number,
            }));

            await mutateAsync({
                body: updates,
            });

            setUpdateSuccess(`Zaktualizowano ${selectedRows.length} paczek.`);
            setSelectedRows([]);
            setNewStatus("");
            refetchParcels();
        } catch {
            setUpdateError("Nie udało się zaktualizować statusów paczek.");
        }
    };

    const columns: GridColDef<ParcelRow>[] = [
        { field: 'friendlyId', headerName: 'ID Paczki', flex: 1.5, minWidth: 200 },
        { field: 'status', headerName: 'Status', flex: 1, minWidth: 150 },
    ];

    const parcels: ParcelRow[] = (parcelsData?.data ?? []).map((p) => ({
        id: p.friendlyId,
        friendlyId: p.friendlyId,
        status: p.status ?? '-'
    }));

    const maczkopat = detailsData?.data as MaczkopatDetails | null | undefined;

    if (detailsLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (detailsError || !maczkopat) {
        return (
            <Box sx={{ p: 2 }}>
                <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/Maczkopats')}>
                    Powrót do listy
                </Button>
                <Typography color="error" sx={{ p: 2, textAlign: 'center' }}>
                    Wystąpił błąd podczas ładowania szczegółów maczkopatu.
                </Typography>
            </Box>
        );
    }

    return (
        <Box sx={{ p: { xs: 1, sm: 2 }, height: '100%', overflow: 'auto' }}>
            <Button 
                startIcon={<ArrowBackIcon />} 
                onClick={() => navigate('/Maczkopats')}
                size="small"
                sx={{ mb: 1 }}
            >
                Powrót
            </Button>

            <Typography variant="h6" gutterBottom sx={{ fontWeight: 'bold' }}>
                Maczkopat: {maczkopat.code}
            </Typography>

            {/* Details section - compact */}
            <Paper elevation={3} sx={{ p: 2, mb: 2 }}>
                <Grid container spacing={1.5}>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Kod</Typography>
                        <Typography variant="body2">{maczkopat.code}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Pojemność</Typography>
                        <Typography variant="body2">{maczkopat.capacity}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Strefa</Typography>
                        <Chip label={maczkopat.areaName} color="primary" size="small" />
                    </Grid>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Miasto</Typography>
                        <Typography variant="body2">{maczkopat.city}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Kod pocztowy</Typography>
                        <Typography variant="body2">{maczkopat.postalCode}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 4, md: 2 }}>
                        <Typography variant="caption" color="textSecondary">Adres</Typography>
                        <Typography variant="body2">{maczkopat.street} {maczkopat.buildingNumber}</Typography>
                    </Grid>
                </Grid>
            </Paper>

            {/* Parcels section */}
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                <Typography variant="subtitle1" fontWeight="bold">
                    Paczki ({parcels.length})
                </Typography>
            </Box>

            {/* Mass status update controls - compact */}
            <Paper elevation={2} sx={{ p: 1.5, mb: 1.5 }}>
                <Stack direction="row" spacing={1.5} alignItems="center" flexWrap="wrap">
                    <Typography variant="body2" sx={{ minWidth: 'fit-content' }}>
                        Zaznaczono: {selectedRows.length}
                    </Typography>
                    <StatusDropdown
                        value={newStatus}
                        onChange={setNewStatus}
                        disabled={selectedRows.length === 0}
                        label="Nowy status"
                    />
                    <Button
                        variant="contained"
                        size="small"
                        onClick={handleMassUpdate}
                        disabled={selectedRows.length === 0 || newStatus === "" || isUpdating}
                        sx={{ 
                            backgroundColor: '#FFDE59', 
                            color: '#323232',
                            '&:hover': {
                                backgroundColor: '#E5C84F',
                            }
                        }}
                    >
                        {isUpdating ? <CircularProgress size={20} /> : 'Zmień status'}
                    </Button>
                </Stack>
                {updateError && <Alert severity="error" sx={{ mt: 1 }}>{updateError}</Alert>}
                {updateSuccess && <Alert severity="success" sx={{ mt: 1 }}>{updateSuccess}</Alert>}
            </Paper>

            {parcelsLoading && (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                    <CircularProgress />
                </Box>
            )}

            {parcelsError && (
                <Typography color="error" sx={{ textAlign: 'center', py: 2 }}>
                    Wystąpił błąd podczas ładowania paczek.
                </Typography>
            )}

            {!parcelsLoading && !parcelsError && (
                <DataGrid
                    rows={parcels}
                    columns={columns}
                    checkboxSelection
                    density="compact"
                    rowSelectionModel={{ type: 'include', ids: new Set(selectedRows) }}
                    onRowSelectionModelChange={handleSelectionChange}
                    initialState={{
                        pagination: { paginationModel: { pageSize: 10 } },
                    }}
                    pageSizeOptions={[10, 25]}
                    disableRowSelectionOnClick
                    sx={{
                        height: 'calc(100vh - 350px)',
                        minHeight: 250,
                        boxShadow: 1,
                        border: 1,
                        borderColor: '#FFDE59',
                        "& .MuiDataGrid-footerContainer": {
                            backgroundColor: "#FFDE59",
                            color: "black",
                        }
                    }}
                />
            )}
        </Box>
    );
};
