import Axios from "axios";
import { Authentication } from "../Security/authentitcation";

Axios.defaults.headers['X-CSRF'] = '1';

export const httpClient = {
  auth: {
    register: async (username: string, password: string): Promise<Authentication> => {
      const { data } = await Axios.post<Authentication>('/api/auth/register', { username, password });
      Authentication.current = data;
      return data;
    },
    login: async (username: string, password: string): Promise<Authentication> => {
      const { data } = await Axios.post<Authentication>('/api/auth/login', { username, password });
      Authentication.current = data;
      return data;
    },
    refresh: async (username: string, refreshToken: string): Promise<Authentication> => {
      const { data } = await Axios.post<Authentication>('/api/auth/refresh', { username, refreshToken });
      Authentication.current = data;
      return data;
    }
  }
}