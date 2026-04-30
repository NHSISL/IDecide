import { SessionData } from "../models/sessions/SessionData";
import ApiBroker from "./apiBroker";

class AuthSessionBroker {
    relativeFeatureUrl = '/auth/session';

    private apiBroker: ApiBroker = new ApiBroker();

    async GetSessionDetailsAsync(): Promise<SessionData> {
        const url = this.relativeFeatureUrl;
        const result = await this.apiBroker.GetAsyncUnauthenticated(url);

        return result.data;
    }

    async PostLogoutAsync(): Promise<void> {
        const response = await fetch('/auth/logout', { method: 'POST' });
        const { logoutUrl } = await response.json();
        window.location.href = logoutUrl;
    }
}

export default AuthSessionBroker;