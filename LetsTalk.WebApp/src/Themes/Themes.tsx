import { Brightness4, Brightness7 } from '@mui/icons-material';
import { Box, IconButton, PaletteMode } from '@mui/material';
import { amber, deepOrange, green, purple } from "@mui/material/colors";
import { useState } from "react";
import { themeManager } from '../Services/theme-manager';

// const getDesignTokens = (mode: PaletteMode) => ({
//   palette: {
//     mode,
//     primary: {
//       ...amber,
//       ...(mode === "dark" && {
//         main: amber[300]
//       })
//     },
//     ...(mode === "dark" && {
//       background: {
//         default: deepOrange[900],
//         paper: deepOrange[900]
//       }
//     }),
//     text: {
//       ...(mode === "light"
//         ? {
//           primary: purple[900],
//           secondary: purple[800]
//         }
//         : {
//           primary: green[500],
//           secondary: green[500]
//         })
//     }
//   }
// });

export const Themes = () => {
  const [themeName, setThemeName] = useState(themeManager.themeName);
  console.log('Name', themeName)
  const toggleMode = () => {
    const name = themeName === 'dark' ? 'light' : 'dark';
    setThemeName(name);
    themeManager.themeName = name;
  };

  return (
    <Box>
      {themeName}
      <IconButton
        onClick={(toggleMode)}>
        {themeName === 'dark' ? <Brightness7 /> : <Brightness4 />}
      </IconButton>
    </Box>
  )
}