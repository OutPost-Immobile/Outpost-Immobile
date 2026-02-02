import React from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    CircularProgress,
    Typography
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { $api } from "../Api/Api.ts";
import { GET_METHOD, MACZKOPAT_PARCELS_URL } from "../Consts.ts";

interface ParcelContentDialogProps {
    open: boolean;
    onClose: () => void;
    maczkopatId: string | null;
    maczkopatCode: string | null;
}

export const ParcelContentDialog: React.FC<ParcelContentDialogProps> = ({ 
    open, 
    onClose, 
    maczkopatId,
    maczkopatCode 
}) => {
    const { data, isLoading, isError } = $api.useQuery(
        GET_METHOD,
        MACZKOPAT_PARCELS_URL,
        {
            params: {
                path: { maczkopatId: maczkopatId as string },
            },
        },
        {
            enabled: !!maczkopatId && open,
        }
    );

    const parcels = data?.data ?? [];

    const columns: GridColDef[] = [
        { field: "index", headerName: "#", width: 70 },
        { field: "friendlyId", headerName: "ID Paczki", flex: 1 },
        { field: "status", headerName: "Status", flex: 1 }
    ];

    const rows = parcels.map((p, index) => ({
        id: p.friendlyId,
        index: index + 1,
        friendlyId: p.friendlyId,
        status: p.status ?? '-'
    }));

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="md"
            fullWidth
        >
            <DialogTitle>
                Zawartość maczkopatu: {maczkopatCode ?? 'Ładowanie...'}
            </DialogTitle>
            <DialogContent dividers sx={{ minHeight: 400 }}>
                {isLoading && (
                    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 300 }}>
                        <CircularProgress />
                    </Box>
                )}
                
                {isError && (
                    <Typography color="error" sx={{ textAlign: 'center', py: 4 }}>
                        Wystąpił błąd podczas ładowania zawartości maczkopatu.
                    </Typography>
                )}
                
                {!isLoading && !isError && (
                    <DataGrid
                        rows={rows}
                        columns={columns}
                        initialState={{
                            pagination: {
                                paginationModel: { pageSize: 10 },
                            },
                        }}
                        pageSizeOptions={[5, 10, 25]}
                        disableRowSelectionOnClick
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
                )}
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} color="primary" variant="contained">
                    Zamknij
                </Button>
            </DialogActions>
        </Dialog>
    );
};
