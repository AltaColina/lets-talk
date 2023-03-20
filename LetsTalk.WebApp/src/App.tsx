import { Box, CssBaseline, Theme, ThemeProvider } from '@mui/material';
import { SnackbarProvider } from 'notistack';
import { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import { attachMessageHandlers } from './Callbacks/attach-message-handlers';
import { Home } from './Components/Home';
import { Login } from './Components/Login';
import { PrivateRoutes } from './Components/PrivateRoutes';
import { messenger } from './Services/messenger';
import { themeManager } from './Services/theme-manager';
import { Themes } from './Components/Themes';

attachMessageHandlers(messenger);

const App = () => {
  const [ theme, setTheme ] = useState(themeManager.theme);
  useEffect(() => {
    const handleThemeChanged: (e: CustomEvent<Theme>) => any = e => setTheme(e.detail);
    return themeManager.addThemeChangedListener(handleThemeChanged);
  })
  return (
    <div className="App">
      <SnackbarProvider anchorOrigin={{horizontal: 'right', vertical: 'bottom'}}>
        <Router>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          {/* <Themes></Themes> */}
          <Routes>
            <Route element={<PrivateRoutes />}>
              <Route element={<Home />} path="/" />
            </Route>
            <Route element={<Login />} path="/login" />
          </Routes>
          </ThemeProvider>
        </Router>
      </SnackbarProvider>
    </div>
  );
}

export default App;