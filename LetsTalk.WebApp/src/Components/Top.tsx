import MenuIcon from '@mui/icons-material/Menu';
import { AppBar, Box, IconButton, Toolbar, Typography } from "@mui/material";
import { useMenuContext } from "../Context/Menu";

export const Top = () => {
  const menuContext = useMenuContext();
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="fixed"
        sx={{
          bgcolor: "green",
        }}>
        <Toolbar>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{
              mr: 2,
              display: {
                md: 'none'
              }
            }}
            onClick={menuContext.toggleMenu}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1, textAlign: "center" }}>
            Let's Talk
          </Typography>
          {/* <Button color="inherit">Login</Button> */}
        </Toolbar>
      </AppBar>
    </Box>
  );
}