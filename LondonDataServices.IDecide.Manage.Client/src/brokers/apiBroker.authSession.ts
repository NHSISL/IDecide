import { SessionData } from "../models/sessions/sessionData";
import ApiBroker from "./apiBroker";

class AuthSessionBroker {
    relativeFeatureUrl = '/api/auth/session';

    private apiBroker: ApiBroker = new ApiBroker();

    async GetSessionDetailsAsync(): Promise<SessionData> {
        const url = this.relativeFeatureUrl;
        const result = await this.apiBroker.GetAsyncUnauthenticated(url);

        return result.data;
    }

    async PostLogoutAsync(): Promise<string> {
        const response = await fetch('/api/auth/logout', { method: 'POST', credentials: 'include' });
        const data = await response.json();

        return data.redirectUrl ?? '/';
    }
}

export default AuthSessionBroker;