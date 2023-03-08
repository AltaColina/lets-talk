import React, { useEffect } from 'react';
import './App.css';
import { messenger } from './Services/messenger';
import { Login } from './Components/Login';
import { Loading } from './Components/Loading';
import { NotFound } from './Components/NotFound';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { PrivateRoutes } from './Components/PrivateRoutes';
import { Lobby } from './Components/Lobby';

const addHandlers = () => {
  messenger.on('connect', m => console.log(m));
  messenger.on('disconnect', m => console.log(m));
  messenger.on('joinchat', m => console.log(m));
  messenger.on('leavechat', m => console.log(m));
  messenger.on('content', m => console.log(m));
};

const removeHandlers = () => {
  messenger.off('connect', m => console.log(m));
  messenger.off('disconnect', m => console.log(m));
  messenger.off('joinchat', m => console.log(m));
  messenger.off('leavechat', m => console.log(m));
  messenger.off('content', m => console.log(m));
}

const App = () => {
  useEffect(() => {
    addHandlers();
    return removeHandlers();
  });
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route element={<PrivateRoutes />}>
            <Route element={<Lobby />} path="/" />
          </Route>
          <Route element={<Login />} path="/login" />
        </Routes>
      </Router>
    </div>
  );
}

export default App;