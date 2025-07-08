import ApiBroker from "./apiBroker";
import { UserAccess } from "../models/userAccess/userAccess";

class UserAccessBroker {
    relativeUserAccessUrl = '/odata/userAccesses';

    private apiBroker: ApiBroker = new ApiBroker();

    async FilterUserAccessAsync(userString: string) {
        if (!userString) {
            return [];
        }

        const url = `${this.relativeUserAccessUrl}?$filter=startswith(email,'${userString}')`;

        return await this.apiBroker.GetAsync(url)
            .then(result => {
                if (result.data && result.data.value) {
                    const userAccesses = result.data.value as UserAccess[];
                    const deduplicatedAddress = [... new Set(userAccesses.map(x => x.email))]
                    const deduplicatedRecords = deduplicatedAddress.map(i => userAccesses.find(y => y.email === i));
                    return deduplicatedRecords;
                }
            })
    }

    async GetAllUserAccessAsync(queryString: string) {
        const url = this.relativeUserAccessUrl + queryString;
        if (queryString === "/") {
            return undefined;
        }
        return await this.apiBroker.GetAsync(url)
            .then(result => {
                if (result.data && result.data.value) {
                   
                    return result.data.value as UserAccess[];
                }
            });
    }
}
export default UserAccessBroker;