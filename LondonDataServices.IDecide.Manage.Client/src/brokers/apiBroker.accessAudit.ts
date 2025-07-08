import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { AccessAudit } from "../models/accessAudit/accessAudit";

export type AccessAuditOdataResponse = {
    data: AccessAudit[],
    nextPage: string
}

class AccessAuditBroker {
    relativeAccessAuditUrl = '/api/accessAudits';
    relativeAccessAuditOdataUrl = '/odata/accessAudits'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): AccessAuditOdataResponse => {
        const data = result.data.value as AccessAudit[];

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async GetAllAccessAuditAsync(queryString: string) {
        const url = this.relativeAccessAuditUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as AccessAudit[]);
    }

    async GetAccessAuditFirstPagesAsync(query: string) {
        const url = this.relativeAccessAuditOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetAccessAuditSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }
}
export default AccessAuditBroker;