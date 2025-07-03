import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { UserAccess } from "../models/userAccess/userAccess";

export type UserAccessOdataResponse = {
    data: UserAccess[],
    nextPage: string
}

class UserAccessBroker {
    relativeUserAccessUrl = '/api/userAccesses';
    relativeUserAccessOdataUrl = '/odata/userAccesses'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) : UserAccessOdataResponse => {
        const data = result.data.value as UserAccess[];

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostUserAccessAsync(userAccess: UserAccess) {
        return await this.apiBroker.PostAsync(this.relativeUserAccessUrl, userAccess)
            .then(result => result.data as UserAccess);
    }

    async PostUserAccessBulkAsync(userAccess: UserAccess) {
        return await this.apiBroker.PostAsync(this.relativeUserAccessUrl + '/bulk', userAccess)
            .then(result => result.data as UserAccess);
    }

    async GetAllUserAccessAsync(queryString: string) {
        const url = this.relativeUserAccessUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as UserAccess[]);
    }

    async GetUserAccessFirstPagesAsync(query: string) {
        const url = this.relativeUserAccessOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetDistinctUsers() {
        const url = this.relativeUserAccessOdataUrl + '?$apply=groupby((UserId,displayName,email))';
        return await this.apiBroker.GetAsync(url).then(result => result.data as UserAccess[]);
    }

    async GetUserAccessSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetLookupByIdAsync(id: string) {
        const url = `${this.relativeUserAccessUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as UserAccess);
    }

    async PutUserAccessAsync(userAccess: UserAccess) {
        return await this.apiBroker.PutAsync(this.relativeUserAccessUrl, userAccess)
            .then(result => result.data as UserAccess);
    }

    async DeleteUserAccessByIdAsync(id: string) {
        const url = `${this.relativeUserAccessUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => result.data as UserAccess);
    }
}
export default UserAccessBroker;