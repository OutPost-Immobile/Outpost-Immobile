import Box from "@mui/material/Box";
import List from "@mui/material/List";
import {Divider, Drawer, ListItem, ListItemButton, ListItemText, Typography} from '@mui/material';
import {Link} from "react-router";

export const DrawerMenu = ({open, onClose}) => {

    const navItems = [
    {
        Text: "Infrastruktura",
        Link: "/Infrastructure"
    },
    {
        Text: "Stan maczkopatu",
        Link: "/Maczkopat"
    },
    {
        Text: "Trasy",
        Link: "/Routes"
    }];

    const drawerContent = (
        <Box
            sx={{ width: 250 }}
            role="presentation"
            onClick={onClose}
            onKeyDown={onClose}
        >
            <Box sx={{ p: 2, backgroundColor: '#FFDE59', height: 120, alignItems: 'center', display: 'flex' }}>
                <Typography variant="h6" component="div">
                    Menu
                </Typography>
            </Box>
            <Divider />
            <List>
                {navItems.map((data) => (
                    <ListItem key={data.Text} disablePadding>
                        <ListItemButton component={Link} to={data.Link}>
                            <ListItemText primary={data.Text}/>
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
            <Divider />
            <List>
                <ListItem disablePadding>
                    <ListItemButton>
                        <ListItemText primary="Settings" />
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
                sx: { width: 250 },
            }}
        >
            {drawerContent}
        </Drawer>
    );
};