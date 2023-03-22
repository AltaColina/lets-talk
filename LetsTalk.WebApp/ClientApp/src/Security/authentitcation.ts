import { User } from "../Users/user";

export interface IAuthentication {
    readonly user: User;
    readonly accessToken: string;
    readonly accessTokenExpires: string;
    readonly refreshToken: string;
    readonly refreshTokenExpires: string;
    readonly permissions: ReadonlyArray<string>;
}

const authKey = 'LetsTalkAuth';

export class Authentication implements IAuthentication {
    public readonly user: User;
    readonly accessToken: string;
    readonly accessTokenExpires: string;
    readonly refreshToken: string;
    readonly refreshTokenExpires: string;
    public readonly permissions: ReadonlyArray<string>;

    public static get current(): Authentication | null {
        return JSON.parse(localStorage.getItem(authKey)!);
    }

    public static set current(value: Authentication | null) {
        if (value)
            localStorage.setItem(authKey, JSON.stringify(value));
        else
            localStorage.removeItem(authKey);
    }

    public constructor(props: IAuthentication) {
        this.user = props.user;
        this.accessToken = props.accessToken;
        this.accessTokenExpires = props.accessTokenExpires;
        this.refreshToken = props.accessToken;
        this.refreshTokenExpires = props.accessTokenExpires;
        this.permissions = props.permissions;
    }
}