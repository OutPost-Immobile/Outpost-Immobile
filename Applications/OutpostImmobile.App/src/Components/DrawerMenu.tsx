import Box from "@mui/material/Box";
import List from "@mui/material/List";
import {Divider, Drawer, ListItem, ListItemButton, ListItemIcon, ListItemText, Typography} from '@mui/material';
import {Link} from "react-router";
import HomeIcon from "@mui/icons-material/Home";
import BuildIcon from "@mui/icons-material/Build";
import InventoryIcon from "@mui/icons-material/Inventory";
import ListAltIcon from "@mui/icons-material/ListAlt";
import RouteIcon from "@mui/icons-material/Route";
import UpdateIcon from "@mui/icons-material/Update";
import SettingsIcon from "@mui/icons-material/Settings";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";

interface DrawerMenuProps {
    open: boolean;
    onClose: () => void;
}

export const DrawerMenu = ({open, onClose}: DrawerMenuProps) => {

    const navItems = [
    {
        Text: "Strona główna",
        Link: "/",
        Icon: HomeIcon
    },
    {
        Text: "Śledź paczkę",
        Link: "/Track",
        Icon: LocalShippingIcon
    },
    {
        Text: "Infrastruktura",
        Link: "/Infrastructure",
        Icon: BuildIcon
    },
    {
        Text: "Maczkopaty",
        Link: "/Maczkopat",
        Icon: InventoryIcon
    },
    {
        Text: "Lista maczkopatów",
        Link: "/Maczkopats",
        Icon: ListAltIcon
    },
    {
        Text: "Trasy",
        Link: "/Routes",
        Icon: RouteIcon
    },
    {
        Text: "Aktualizacja statusu paczki",
        Link: "/Parcels/UpdateStatus",
        Icon: UpdateIcon
    }];

    const drawerContent = (
        <Box
            sx={{ width: 280 }}
            role="presentation"
            onClick={onClose}
            onKeyDown={onClose}
        >
            <Box sx={{ p: 2, backgroundColor: '#FFDE59', height: 120, alignItems: 'center', display: 'flex' }}>
                <Typography variant="h6" component="div" sx={{ fontWeight: 'bold' }}>
                    Outpost Immobile
                </Typography>
            </Box>
            <Divider />
            <List>
                {navItems.map((data) => (
                    <ListItem key={data.Text} disablePadding>
                        <ListItemButton component={Link} to={data.Link}>
                            <ListItemIcon>
                                <data.Icon sx={{ color: '#FFDE59' }} />
                            </ListItemIcon>
                            <ListItemText primary={data.Text}/>
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
            <Divider />
            <List>
                <ListItem disablePadding>
                    <ListItemButton>
                        <ListItemIcon>
                            <SettingsIcon />
                        </ListItemIcon>
                        <ListItemText primary="Ustawienia" />
                    </ListItemButton>
                </ListItem>
            </List>
        </Box>
    );

    return (
        <Drawer
            anchor="left"
            open={open}
            onClose={onClose}
            PaperProps={{
                sx: { width: 280 },
            }}
        >
            {drawerContent}
        </Drawer>
    );
};