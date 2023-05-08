import { Navigate, Outlet } from 'react-router-dom';
import { hubClient } from '../Services/hub-client';

export const PrivateRoutes = () => {
  //const token = Authentication.current?.accessToken.id;
  return (hubClient.isConnected ? <Outlet /> : <Navigate to="/test" />);
}