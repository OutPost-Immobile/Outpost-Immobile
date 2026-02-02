import { useState } from "react";
import { useNavigate } from "react-router";
import {
    Box,
    Typography,
    CircularProgress,
    IconButton,
    Menu,
    MenuItem,
    ListItemIcon,
    ListItemText
} from "@mui/material";
import {
    DataGrid,
    type GridColDef,
    type GridRenderCellParams
} from "@mui/x-data-grid";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import VisibilityIcon from "@mui/icons-material/Visibility";
import InfoIcon from "@mui/icons-material/Info";
import { $api } from "../Api/Api.ts";
import { GET_METHOD, MACZKOPATS_URL } from "../Consts.ts";
import { ParcelContentDialog } from "../Components/ParcelContentDialog.tsx";

interface MaczkopatRow {
    id: string;
    code: string;
    capacity: number;
    areaName: string;
    city: string;
    street: string;
    buildingNumber: string;
    parcelCount: number;
}

export const MaczkopatListPage = () => {
    const navigate = useNavigate();
    const { data, isLoading, isError } = $api.useQuery(GET_METHOD, MACZKOPATS_URL);
    
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [selectedMaczkopat, setSelectedMaczkopat] = useState<MaczkopatRow | null>(null);
    
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogMaczkopatId, setDialogMaczkopatId] = useState<string | null>(null);
    const [dialogMaczkopatCode, setDialogMaczkopatCode] = useState<string | null>(null);

    const handleMenuOpen = (event: React.MouseEvent<HTMLElement>, row: MaczkopatRow) => {
        setAnchorEl(event.currentTarget);
        setSelectedMaczkopat(row);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
        setSelectedMaczkopat(null);
    };

    const handleViewContent = () => {
        if (selectedMaczkopat) {
            setDialogMaczkopatId(selectedMaczkopat.id);
            setDialogMaczkopatCode(selectedMaczkopat.code);
            setDialogOpen(true);
        }
        handleMenuClose();
    };

    const handleGoToDetails = () => {
        if (selectedMaczkopat) {
            navigate(`/Maczkopat/${selectedMaczkopat.id}`);
        }
        handleMenuClose();
    };

    const handleDialogClose = () => {
        setDialogOpen(false);
        setDialogMaczkopatId(null);
        setDialogMaczkopatCode(null);
    };

    const columns: GridColDef<MaczkopatRow>[] = [
        { field: 'code', headerName: 'Kod', width: 120 },
        { field: 'areaName', headerName: 'Strefa', flex: 1, minWidth: 120 },
        { field: 'city', headerName: 'Miasto', flex: 1, minWidth: 120 },
        { 
            field: 'address', 
            headerName: 'Adres', 
            flex: 1.5, 
            minWidth: 180,
            valueGetter: (_value, row) => `${row.street} ${row.buildingNumber}`
        },
        { 
            field: 'capacity', 
            headerName: 'Pojemność', 
            width: 110,
            type: 'number'
        },
        { 
            field: 'parcelCount', 
            headerName: 'Paczki', 
            width: 100,
            type: 'number'
        },
        {
            field: 'actions',
            headerName: 'Opcje',
            sortable: false,
            width: 80,
            renderCell: (params: GridRenderCellParams<MaczkopatRow>) => (
                <IconButton
                    size="small"
                    onClick={(e) => handleMenuOpen(e, params.row)}
                >
                    <MoreVertIcon />
                </IconButton>
            ),
        },
    ];

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (isError) {
        return (
            <Typography color="error" sx={{ p: 2, textAlign: 'center' }}>
                Wystąpił błąd podczas ładowania listy maczkopatów.
            </Typography>
        );
    }

    const rows: MaczkopatRow[] = (data?.data ?? []).map((m) => ({
        id: m.id,
        code: m.code,
        capacity: Number(m.capacity),
        areaName: m.areaName,
        city: m.city,
        street: m.street,
        buildingNumber: m.buildingNumber,
        parcelCount: m.parcelCount
    }));

    return (
        <Box sx={{ height: 'calc(100vh - 80px)', width: '100%', p: { xs: 1, sm: 2 }, display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h6" gutterBottom sx={{ fontWeight: 'bold', mb: 1 }}>
                Lista maczkopatów
            </Typography>

            <DataGrid
                rows={rows}
                columns={columns}
                getRowId={(row) => row.id}
                density="compact"
                initialState={{
                    pagination: { paginationModel: { pageSize: 15 } },
                }}
                pageSizeOptions={[15, 25, 50]}
                disableRowSelectionOnClick
                sx={{
                    flex: 1,
                    boxShadow: 1,
                    border: 1,
                    borderColor: '#FFDE59',
                    "& .MuiDataGrid-footerContainer": {
                        backgroundColor: "#FFDE59",
                        color: "black",
                    }
                }}
            />
            <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'right',
                }}
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
            >
                <MenuItem onClick={handleViewContent}>
                    <ListItemIcon>
                        <VisibilityIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Pokaż zawartość</ListItemText>
                </MenuItem>
                <MenuItem onClick={handleGoToDetails}>
                    <ListItemIcon>
                        <InfoIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Szczegóły maczkopatu</ListItemText>
                </MenuItem>
            </Menu>
            <ParcelContentDialog
                open={dialogOpen}
                onClose={handleDialogClose}
                maczkopatId={dialogMaczkopatId}
                maczkopatCode={dialogMaczkopatCode}
            />
        </Box>
    );
};
