import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { ImpersonationContext } from "../models/impersonationContext/impersonationContext";

class ImpersonationContextBroker {
    relativeImpersonationContextUrl = '/api/impersonationContexts';
    relativeImpersonationContextOdataUrl = '/odata/impersonationContexts'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value as ImpersonationContext[];

        const nextPage = result.data['@odata.nextLink'] as string;
        return { data, nextPage }
    }

    async PostImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PostAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => result.data as ImpersonationContext);
    }

    async GetAllImpersonationContextAsync(queryString: string) {
        const url = this.relativeImpersonationContextUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as ImpersonationContext[]);
    }

    async GetImpersonationContextFirstPagesAsync(query: string) {
        const url = this.relativeImpersonationContextOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetImpersonationContextSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetImpersonationContextByIdAsync(id: string) {
        const url = `${this.relativeImpersonationContextUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as ImpersonationContext);
    }

    async PutImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PutAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => result.data as ImpersonationContext);
    }

    async DeleteImpersonationContextByIdAsync(id: string) {
        const url = `${this.relativeImpersonationContextUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => result.data as ImpersonationContext);
    }
}
export default ImpersonationContextBroker;