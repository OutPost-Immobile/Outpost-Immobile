import {$api} from "../Api/Api.ts";
import {GET_METHOD, ROUTE_URL} from "../Consts.ts";
import {
    DataGrid,
    type GridColDef,
    type GridRenderCellParams
} from "@mui/x-data-grid";
import {
    Button,
    Typography,
    Box,
    CircularProgress
} from "@mui/material";
import MapIcon from "@mui/icons-material/Map";
import { RouteMapDialog } from "../Components/MapDialog.tsx";
import {useState} from "react";

export const RoutesPage = () => {
    const { data, isLoading, isError } = $api.useQuery(GET_METHOD, ROUTE_URL);
    const [selectedRoute, setSelectedRoute] = useState<any | null>(null);

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
            headerName: 'Mapa',
            sortable: false,
            width: 120,
            renderCell: (params: GridRenderCellParams) => (
                <Button
                    variant="outlined"
                    size="small"
                    startIcon={<MapIcon />}
                    onClick={() => setSelectedRoute(params.row)}
                >
                    Zobacz
                </Button>
            ),
        },
    ];

    if (isLoading) {
        return <Box sx={{display: 'flex', justifyContent: 'center', p: 5}}><CircularProgress/></Box>;
    }

    if (isError) {
        return <Typography color="error" sx={{ p: 2 }}>Error loading routes.</Typography>;
    }

    return (
        <Box sx={{ height: 'calc(100vh - 100px)', width: '100%', p: 2 }}>
            <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
                Dostępne trasy
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
            />

            <RouteMapDialog
                open={Boolean(selectedRoute)}
                onClose={() => setSelectedRoute(null)}
                route={selectedRoute}
            />
        </Box>
    );
};