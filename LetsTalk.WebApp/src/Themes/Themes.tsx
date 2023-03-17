import { createTheme, PaletteMode, IconButton, Box } from '@mui/material';
import { amber, deepOrange, purple, green } from "@mui/material/colors";
import { Brightness4, Brightness7 } from '@mui/icons-material';
import { themeManager } from '../Services/theme-manager';
import { useState } from "react";

const getDesignTokens = (mode: PaletteMode) => ({
    palette: {
      mode,
      primary: {
        ...amber,
        ...(mode === "dark" && {
          main: amber[300]
        })
      },
      ...(mode === "dark" && {
        background: {
          default: deepOrange[900],
          paper: deepOrange[900]
        }
      }),
      text: {
        ...(mode === "light"
          ? {
              primary: purple[900],
              secondary: purple[800]
            }
          : {
              primary: green[500],
              secondary: green[500]
            })
      }
    }
});

export const Themes = () => {
    const [ mode, setMode ] = useState(themeManager.themeMode);
    const toggleMode = () => {
            setMode(mode === 'dark' ? 'light' : 'dark');
    };

    return (
        <Box>
            { mode }
            <IconButton
                onClick={(toggleMode)}>
                { mode === 'dark' ? <Brightness7 /> : <Brightness4 /> }
            </IconButton>
        </Box>
    )
}