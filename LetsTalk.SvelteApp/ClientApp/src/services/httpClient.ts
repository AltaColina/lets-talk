import { UserProfile } from "../models/security/userProfile.js";
import type { User } from "../models/users/user.js";

const setDefaults = (method: string, init?: RequestInit): RequestInit => {
    return {
        ...init,
        method,
        headers: {
            ...init?.headers,
            'X-CSRF': '1'
        }
    };
}

const http = {
    get(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('GET', init));
    },

    post(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('POST', init));
    },

    put(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('PUT', init));
    },

    delete(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('DELETE', init));
    },
};


class BffApi {
    async user(): Promise<UserProfile> {
        const response = await http.get('/bff/user');
        if (response.ok) {
            return new UserProfile(await response.json());
        }
        return UserProfile.Guest;
    }
}

class GreetApi {
    get(): Promise<string> {
        return http.get('/api/greet').then(r => r.text());
    }
}

class UserApi {
    get(roles?: string[]): Promise<{ users: User[] }>
    get(userId: string): Promise<User | undefined>
    get(args?: string | string[]): Promise<{ users: User[] } | User | undefined> {
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
        return http.get(uri).then(r => r.json());
    }

    post(user: { userName: string, email: string, password: string }): Promise<User> {
        return http.post('/api/user', { body: JSON.stringify(user) }).then(r => r.json());
    }

    // fix this.
    //async put(user: { })

    delete(userId: string) {
        return http.delete(`/api/user/${userId}`);
    }
}

export const bffApi = new BffApi();
export const greetApi = new GreetApi();
export const userApi = new UserApi();