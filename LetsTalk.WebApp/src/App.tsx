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
      <Router>
        <Routes>
          <Route element={<PrivateRoutes />}>
            <Route element={<Home />} path="/" />
          </Route>
          <Route element={<Login />} path="/login" />
        </Routes>
      </Router>
    </div>
  );
}

export default App;