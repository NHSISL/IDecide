import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { ImpersonationContext } from "../models/impersonationContext/impersonationContext";

type ImpersonationContextODataResponse = {
    data : ImpersonationContext[],
    nextPage: string
}

class ImpersonationContextBroker {
    relativeImpersonationContextUrl = '/api/impersonationContexts';
    relativeImpersonationContextOdataUrl = '/odata/impersonationContexts'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) : ImpersonationContextODataResponse =>  {
        const data = result.data.value.map((impersonationContext: ImpersonationContext) => new ImpersonationContext(impersonationContext));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PostAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => new ImpersonationContext(result.data));
    }

    async GetAllImpersonationContextAsync(queryString: string) {
        const url = this.relativeImpersonationContextUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((impersonationContext: ImpersonationContext) => new ImpersonationContext(impersonationContext)));
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
            .then(result => new ImpersonationContext(result.data));
    }

    async PutImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PutAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => new ImpersonationContext(result.data));
    }

    async DeleteImpersonationContextByIdAsync(id: string) {
        const url = `${this.relativeImpersonationContextUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new ImpersonationContext(result.data));
    }
}
export default ImpersonationContextBroker;