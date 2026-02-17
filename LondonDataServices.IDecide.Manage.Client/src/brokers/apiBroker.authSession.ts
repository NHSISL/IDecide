import { Feature } from "../models/features/feature";
import ApiBroker from "./apiBroker";

class AuthSessionBroker {
    relativeFeatureUrl = '/auth/session';

    private apiBroker: ApiBroker = new ApiBroker();

    //async GetSessionDetailsAsync(): Promise<Session[]> {
    //    const url = this.relativeFeatureUrl;
    //    const result = await this.apiBroker.GetAsync(url);
    //    return result.data.map((session: any) => new Session(session));
    //}

    //async CheckIsAuthenticatedAsync(): Promise<boolean> {
    //    try {
    //        const url = this.relativeFeatureUrl;
    //        const result = await this.apiBroker.GetAsync(url);
    //        return result.status === 200;
    //    } catch {
    //        return false;
    //    }
    //}
}
export default AuthSessionBroker;