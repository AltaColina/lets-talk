import { Brightness4, Brightness7 } from '@mui/icons-material';
import { Box, IconButton, useMediaQuery } from '@mui/material';
import { useState } from "react";
import { themeManager } from '../Services/theme-manager';

export const Themes = () => {
  
  //? Implement auto dark
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  console.warn('User prefers darkmode?', prefersDarkMode)
  
  const [themeName, setThemeName] = useState(themeManager.themeName);
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