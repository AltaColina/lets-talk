import Axios from "axios";
import { Claim } from "../Security/claim";

Axios.defaults.headers['X-CSRF'] = '1';

export const httpClient = {
  bff: {
    user: async (): Promise<ReadonlyArray<Claim> | undefined> => {
      return await Axios.get<ReadonlyArray<Claim>>('/bff/user').then(r => r.data);
    }
  },
  ping: {
    get: async (): Promise<string> => {
      return await Axios.get<string>('/api/ping').then(r => r.data);
    }
  }
}