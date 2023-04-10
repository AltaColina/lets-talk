import { CssBaseline, ThemeProvider } from '@mui/material';
import { SnackbarProvider } from 'notistack';
import { useEffect, useState } from "react";
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { HelmetProvider } from 'react-helmet-async';
import './App.css';
import attachDebugListeners from './Callbacks/attach-debug-listeners';
import { Login } from './Components/Login';
import { PrivateRoutes } from './Components/PrivateRoutes';
import { messenger } from './Services/messenger';
import { themeManager } from './Services/theme-manager';
import Page404 from './Pages/404';
import Layout from './Pages/Layout';
import Test from './Pages/Test';
import { Room } from './Components/Room';
import Landing from './Pages/Landing';

attachDebugListeners(messenger);

const App = () => {
    const [theme, setTheme] = useState(themeManager.theme);
    useEffect(() => messenger.on('ThemeChanged', e => setTheme(e.detail)).dispose);
    return (
        <HelmetProvider>
            <SnackbarProvider anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}>
                <ThemeProvider theme={theme}>
                    <CssBaseline />
                    <Router>
                        <Routes>
                            <Route element={<PrivateRoutes />}>
                                <Route element={<Layout />} path="/">
                                    <Route element={<Room />} index />
                                    {/*<Route element={<Test />} path="/test" />*/}
                                </Route>
                            </Route>
                            <Route element={<Landing />} path="/">
                                {/*<Route element={<Login />} path="/login" />*/}
                                <Route element={<Test />} path="/test" />
                            </Route>
                            <Route element={<Page404 />} path="*" />
                        </Routes>
                    </Router>
                </ThemeProvider>
            </SnackbarProvider>
        </HelmetProvider>
    );
}

export default App;