import {AppBar, CssBaseline, Divider, IconButton, Toolbar, Typography} from "@mui/material";
import { Outlet } from "react-router";
import {DrawerMenu} from "../Components/DrawerMenu.tsx";
import MenuIcon from '@mui/icons-material/Menu';
import {useState} from "react";
import OutpostLogo from '../assets/Outpost.png';
import Box from "@mui/material/Box";
import {Link} from "react-router";

export const Layout = () => {

    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => (event) => {
        if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
            return;
        }
        setDrawerOpen(open);
    };

    return (
        <>
            <CssBaseline />
            <div className="flex align-top bg-yellow text-[#323232]">
                {process.env.NODE_ENV === 'development' && (
                    <div className='absolute top-4 right-4'>

                    </div>
                )}
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
                           {/* Nieresponsywne*/}
                           <Divider orientation="vertical" style={{width: 1300, visibility: 'hidden'}} flexItem/>
                           <Typography variant="h6" style={
                               {display: 'inline-block',
                                verticalAlign: 'middle',
                                color: 'black',
                                textAlign: 'right'}}>
                               Placeholder na konto klienta
                           </Typography>
                        </Toolbar>
                    </AppBar>
                </div>
                {/*<Toolbar className='sm:hidden' />*/}
                <main style={{ paddingTop: 120 }}>
                    <Outlet />
                </main>
            </div>
        </>
    );
};