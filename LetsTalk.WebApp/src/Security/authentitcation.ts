import { User } from "../Users/user";
import { Token } from "./token";

export interface IAuthentication {
    readonly user: User;
    readonly accessToken: Token;
    readonly refreshToken: Token;
    readonly permissions: ReadonlyArray<string>;
}

const authKey = 'LetsTalkAuth';

export class Authentication implements IAuthentication {
    public readonly user: User;
    public readonly accessToken: Token;
    public readonly refreshToken: Token;
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
        this.refreshToken = props.accessToken;
        this.permissions = props.permissions;
    }
}