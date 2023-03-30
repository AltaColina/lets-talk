import Axios from "axios";
import { Claim } from "../Security/claim";
import { User } from "../Users/user";

Axios.defaults.headers['X-CSRF'] = '1';

class BffApi {
  async user(): Promise<ReadonlyArray<Claim> | undefined> {
    return await Axios.get<ReadonlyArray<Claim>>('/bff/user').then(r => r.data);
  }
}

class PingApi {
  async get(): Promise<string> {
    return await Axios.get<string>('/api/ping').then(r => r.data);
  }
}

class UserApi {
  async get(roles?: string[]): Promise<{ users: User[] }>
  async get(userId: string): Promise<User | undefined>
  async get(args?: string | string[]): Promise<{ users: User[] } | User | undefined> {
    let uri = '/api/user';
    if (typeof args === 'string') {
      uri += `/${args}`;
    }
    else if (Array.isArray(args) && args.length > 0) {
      uri += `?role=${args[0]}`;
      for (let i = 1; i < args.length; ++i) {
        uri += `&role=${args[i]}`;
      }
    }
    return await Axios.get<{ users: User[] } | User | undefined>(uri).then(r => r.data);
  }

  async post(user: { userName: string, email: string, password: string}): Promise<User> {
    return await Axios.post<User>('/api/user', user).then(r => r.data);
  }
  
  // fix this.
  //async put(user: { })

  async delete(userId: string) {
    return await Axios.delete(`/api/user/${userId}`).then(r => r.data);
  }
}

class HttpClient {
  readonly bff = new BffApi();
  readonly ping = new PingApi();
  readonly user = new UserApi();
}

export const httpClient = new HttpClient();