import {Button, Stack, Paper} from "@mui/material";

export const MaczkopatPage = () => {
    
    return (
        <Stack spacing={2} style={{justifyContent: 'center', alignItems: 'center', paddingTop: 64}}>
            <Paper elevation={6} style={{padding: 16, margin: 64}}>
                <Button variant="outlined">
                    papieżak polak 2137
                </Button>
            </Paper>
        </Stack>    
    )
}