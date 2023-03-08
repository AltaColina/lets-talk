import { Outlet, Navigate } from 'react-router-dom';
import { Authentication } from '../Security/authentitcation';
import { hubClient } from '../Services/hub-client';

export const PrivateRoutes = () => {
  //const token = Authentication.current?.accessToken.id;
  return (hubClient.isConnected ? <Outlet /> : <Navigate to="/login" />);
}