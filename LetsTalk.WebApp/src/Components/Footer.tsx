import { AppBar, Typography } from "@mui/material";

export const Footer = () => {
    return (
        <AppBar
            position="fixed"
            sx={{
                top: 'auto',
                bottom: 0,
                p: 1,
                bgcolor: "green",
            }}>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1, textAlign: "center" }}>
                This will be a footer | Copyright stuff
            </Typography>
        </AppBar>
    );
}