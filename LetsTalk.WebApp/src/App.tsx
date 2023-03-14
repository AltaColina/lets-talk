import { SnackbarProvider } from 'notistack';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import { attachMessageHandlers } from './Callbacks/attach-message-handlers';
import { Home } from './Components/Home';
import { Login } from './Components/Login';
import { PrivateRoutes } from './Components/PrivateRoutes';
import { messenger } from './Services/messenger';

attachMessageHandlers(messenger);

const App = () => {
  return (
    <div className="App">
      <SnackbarProvider anchorOrigin={{horizontal: 'right', vertical: 'bottom'}}>
        <Router>
          <Routes>
            <Route element={<PrivateRoutes />}>
              <Route element={<Home />} path="/" />
            </Route>
            <Route element={<Login />} path="/login" />
          </Routes>
        </Router>
      </SnackbarProvider>
    </div>
  );
}

export default App;