import {AppBar, CssBaseline, Divider, IconButton, Toolbar, Typography, Button} from "@mui/material";
import { Outlet } from "react-router";
import {DrawerMenu} from "../Components/DrawerMenu.tsx";
import MenuIcon from '@mui/icons-material/Menu';
import {useState} from "react";
import OutpostLogo from '../assets/Outpost.png';
import Box from "@mui/material/Box";
import {Link} from "react-router";
import {useAuth} from "../Auth/AuthProvider.tsx";
import {jwtDecode} from "jwt-decode";

export const Layout = () => {
    const { isAuthenticated, token, logout } = useAuth();
    const [drawerOpen, setDrawerOpen] = useState(false);

    let userEmail = "";
    if (token) {
        try {
            const decoded: any = jwtDecode(token);
            userEmail = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || decoded.email || decoded.sub || "";
        } catch (e) {
            console.error("Token decode error", e);
        }
    }

    const toggleDrawer = (open: boolean) => (event) => {
        if (event && event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
            return;
        }
        setDrawerOpen(open);
    };

    return (
        <>
            <CssBaseline />
            <div className="flex align-top bg-yellow text-[#323232]">
                <div className='block sm:hidden'>
                    <DrawerMenu open={drawerOpen} onClose={toggleDrawer(false)}/>
                </div>
                <div className='sm:hidden z-40 fixed'>
                    <AppBar sx={{backgroundColor: '#FFDE59'}} position='fixed'>
                        <Toolbar className='flex justify-between w-full'>
                            <IconButton
                                edge="start"
                                color="inherit"
                                aria-label="menu"
                                onClick={toggleDrawer(true)}
                                sx={{ color: '#323232',
                                    marginRight: 1,
                                    height: 120 }}>
                                <MenuIcon />
                            </IconButton>
                            <Link to={"/"}>
                                <Box className='flex-grow justify-center' sx={{ display: 'flex', alignItems: 'center'}}>
                                    <img src={OutpostLogo} alt="Outpost" style={{width: 120, height: 120}}/>
                                    <Typography variant="h6" style={{display: 'inline-block', verticalAlign: 'middle', color: 'black'}}>Outpost Immobile</Typography>
                                </Box>
                            </Link>
                            <Divider orientation="vertical" style={{flexGrow: 1, visibility: 'hidden'}} flexItem/>

                            {isAuthenticated ? (
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                    <Typography variant="body2" sx={{ color: 'black', fontWeight: 'bold' }}>
                                        {userEmail}
                                    </Typography>
                                    <Button
                                        onClick={logout}
                                        sx={{ color: 'black', border: '1px solid black' }}
                                    >
                                        Wyloguj
                                    </Button>
                                </Box>
                            ) : (
                                <Link to="/Login" style={{textDecoration: 'none'}}>
                                    <Typography variant="h6" style={{
                                        display: 'inline-block',
                                        verticalAlign: 'middle',
                                        color: 'black',
                                        textAlign: 'right'
                                    }}>
                                        Zaloguj się
                                    </Typography>
                                </Link>
                            )}
                        </Toolbar>
                    </AppBar>
                </div>
                <main style={{ paddingTop: 120, width: '100%' }}>
                    <Outlet />
                </main>
            </div>
        </>
    );
};