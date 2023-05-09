
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

class Http {
    get(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('GET', init));
    }

    post(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('POST', init));
    }

    put(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('PUT', init));
    }

    delete(url: string, init?: RequestInit): Promise<Response> {
        return fetch(url, setDefaults('DELETE', init));
    }
}

export const http = new Http();