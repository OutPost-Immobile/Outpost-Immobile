import { $api } from "../Api/Api.ts";
import { GET_METHOD, ROUTE_URL, ROUTE_BY_COURIER_URL } from "../Consts.ts";
import {
    DataGrid,
    type GridColDef,
    type GridRenderCellParams
} from "@mui/x-data-grid";
import {
    Button,
    Typography,
    Box,
    CircularProgress,
    Stack
} from "@mui/material";
import MapIcon from "@mui/icons-material/Map";
import CalculateIcon from "@mui/icons-material/Calculate";
import { RouteMapDialog } from "../Components/MapDialog.tsx";
import { CalculateDistanceDialog } from "../Components/CalculateDistanceDialog.tsx";
import { useState } from "react";
import { getUserInfoFromToken, isCourier } from "../Helpers/JwtHelper.ts";

export const RoutesPage = () => {
    const userInfo = getUserInfoFromToken();
    const isUserCourier = isCourier();
    const courierId = userInfo.userId;

    const courierQuery = $api.useQuery(
        GET_METHOD,
        ROUTE_BY_COURIER_URL,
        {
            params: {
                path: { courierId: courierId as string },
            },
        },
        {
            enabled: isUserCourier && !!courierId,
        }
    );

    const allRoutesQuery = $api.useQuery(
        GET_METHOD,
        ROUTE_URL,
        {},
        {
            enabled: !isUserCourier,
        }
    );

    const data = isUserCourier ? courierQuery.data : allRoutesQuery.data;
    const isLoading = isUserCourier ? courierQuery.isLoading : allRoutesQuery.isLoading;
    const isError = isUserCourier ? courierQuery.isError : allRoutesQuery.isError;
    const refetch = isUserCourier ? courierQuery.refetch : allRoutesQuery.refetch;

    const [selectedRoute, setSelectedRoute] = useState<any | null>(null);
    const [calculateRouteId, setCalculateRouteId] = useState<number | null>(null);

    const columns: GridColDef[] = [
        { field: 'routeId', headerName: 'ID', width: 90 },
        { field: 'startAddressName', headerName: 'Początkowa lokacja', flex: 1 },
        { field: 'endAddressName', headerName: 'Końcowa lokacja', flex: 1 },
        {
            field: 'distance',
            headerName: 'Dystans',
            width: 130,
            valueGetter: (value) => value ? `${(Number(value) / 1000).toFixed(2)} km` : '-'
        },
        {
            field: 'actions',
            headerName: 'Akcje',
            sortable: false,
            width: 220,
            renderCell: (params: GridRenderCellParams) => (
                <Stack direction="row" spacing={1}>
                    <Button
                        variant="outlined"
                        size="small"
                        startIcon={<MapIcon />}
                        onClick={() => setSelectedRoute(params.row)}
                    >
                        Mapa
                    </Button>
                    <Button
                        variant="outlined"
                        size="small"
                        startIcon={<CalculateIcon />}
                        onClick={() => setCalculateRouteId(params.row.routeId)}
                        sx={{
                            borderColor: '#FFDE59',
                            color: '#323232',
                            '&:hover': {
                                borderColor: '#E5C84F',
                                backgroundColor: 'rgba(255, 222, 89, 0.1)'
                            }
                        }}
                    >
                        Oblicz
                    </Button>
                </Stack>
            ),
        },
    ];

    if (isLoading) {
        return <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}><CircularProgress /></Box>;
    }

    if (isError) {
        return <Typography color="error" sx={{ p: 2 }}>Błąd podczas ładowania tras.</Typography>;
    }

    return (
        <Box sx={{ height: 'calc(100vh - 100px)', width: '100%', p: 2 }}>
            <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
                {isUserCourier ? 'Twoje przypisane trasy' : 'Dostępne trasy'}
            </Typography>

            <DataGrid
                rows={data?.data ?? []}
                columns={columns}
                getRowId={(row) => row.routeId}
                initialState={{
                    pagination: { paginationModel: { pageSize: 10 } },
                }}
                pageSizeOptions={[10, 25, 50]}
                disableRowSelectionOnClick
                sx={{
                    boxShadow: 1,
                    border: 1,
                    borderColor: '#FFDE59',
                    "& .MuiDataGrid-footerContainer": {
                        backgroundColor: "#FFDE59",
                        color: "black",
                    }
                }}
            />

            <RouteMapDialog
                open={Boolean(selectedRoute)}
                onClose={() => setSelectedRoute(null)}
                route={selectedRoute}
            />

            <CalculateDistanceDialog
                open={Boolean(calculateRouteId)}
                onClose={() => setCalculateRouteId(null)}
                routeId={calculateRouteId}
                onSaveSuccess={() => refetch()}
            />
        </Box>
    );
};