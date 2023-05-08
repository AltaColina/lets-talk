import { httpClient } from "../Services/http-client";
import { Claim } from "./claim";

export class UserProfile {
    static readonly guestProfile = new UserProfile([{ type: 'name', value: 'guest' }]);
    static get currentProfile() {
        // Avoid calling the BFF.
        // if(!Cookies.get('letstalk_webapp')) {
        //     return this.guestProfile;
        // }
        return httpClient.bff.user().then(claims => claims ? new UserProfile(claims) : this.guestProfile);
    }

    readonly claims: ReadonlyArray<Claim>;
    get subjectId() { return this.claims?.find(c => c.type === 'sub')?.value; }
    get displayName() { return this.claims?.find(c => c.type === 'name')?.value; }
    get picture() { return this.claims?.find(c => c.type === 'picture')?.value; }
    get logoutUrl() { return this.claims?.find(c => c.type === 'bff:logout_url')?.value; }
    get isLoggedIn() { return typeof this.logoutUrl === 'string'; }
    
    private constructor(claims: ReadonlyArray<Claim>) {
        this.claims = claims;
    }
}