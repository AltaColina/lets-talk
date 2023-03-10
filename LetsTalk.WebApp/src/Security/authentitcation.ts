import { User } from "../Users/user";

export interface IAuthentication {
    readonly user: User;
    readonly accessToken: string;
    readonly accessTokenExpiresIn: string;
    readonly refreshToken: string;
    readonly refreshTokenExpiresIn: string;
    readonly permissions: ReadonlyArray<string>;
}

const authKey = 'LetsTalkAuth';

export class Authentication implements IAuthentication {
    public readonly user: User;
    readonly accessToken: string;
    readonly accessTokenExpiresIn: string;
    readonly refreshToken: string;
    readonly refreshTokenExpiresIn: string;
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
        this.accessTokenExpiresIn = props.accessTokenExpiresIn;
        this.refreshToken = props.accessToken;
        this.refreshTokenExpiresIn = props.accessTokenExpiresIn;
        this.permissions = props.permissions;
    }
}