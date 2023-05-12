import type { Claim } from "./claim";

export class UserProfile {
    static readonly Guest = new UserProfile();

    readonly #claims: ReadonlyArray<Claim>;

    get subjectId() { return this.#claims?.find(c => c.type === 'sub')?.value; }
    get displayName() { return this.#claims?.find(c => c.type === 'name')?.value; }
    get picture() { return this.#claims?.find(c => c.type === 'picture')?.value; }
    get logoutUrl() { return this.#claims?.find(c => c.type === 'bff:logout_url')?.value; }
    get isLoggedIn() { return typeof this.logoutUrl === 'string'; }

    constructor(claims?: ReadonlyArray<Claim>) {
        this.#claims = claims || [{ type: 'name', value: 'guest' }];
    }
}