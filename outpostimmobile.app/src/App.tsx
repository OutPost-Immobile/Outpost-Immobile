import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {SnackbarProvider} from "notistack";
import {CssBaseline} from "@mui/material";
import routings from "./Helpers/RoutesProvider.tsx";
import {RouterProvider} from "react-router";
import React from "react";
import ReactDOM from 'react-dom/client'

const queryClient = new QueryClient()

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <QueryClientProvider client={queryClient}>
            <SnackbarProvider anchorOrigin={{ horizontal: 'center', vertical: 'top' }}>
                    <CssBaseline />
                    <RouterProvider router={routings} />
            </SnackbarProvider>
        </QueryClientProvider>
    </React.StrictMode>
)