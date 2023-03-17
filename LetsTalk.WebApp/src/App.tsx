import { SnackbarProvider } from 'notistack';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import { attachMessageHandlers } from './Callbacks/attach-message-handlers';
import { Home } from './Components/Home';
import { Login } from './Components/Login';
import { PrivateRoutes } from './Components/PrivateRoutes';
import { messenger } from './Services/messenger';
import { ThemeProvider, CssBaseline, Theme } from '@mui/material';
import { Themes } from './Themes/Themes';
import { themeManager } from './Services/theme-manager';
import { useState, useEffect } from "react";

attachMessageHandlers(messenger);

const App = () => {
  const [ mode, setMode ] = useState(themeManager.themeMode);
  useEffect(() => {
    const handleThemeChanged: (e: CustomEvent<string>) => any = e => setMode(e.detail);
    return themeManager.addThemeChangedListener(handleThemeChanged);
  })
  return (
    <div className="App">
      <SnackbarProvider anchorOrigin={{horizontal: 'right', vertical: 'bottom'}}>
        <Router>
        <ThemeProvider theme={themeManager.theme(mode)}>
          <CssBaseline />
          <Themes></Themes>
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