import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { Lookup } from "../models/lookups/lookup";

type LookupODataResponse = {
    data : Lookup[],
    nextPage: string
}

class LookupBroker {
    relativeLookupUrl = '/api/lookups';
    relativeLookupOdataUrl = '/odata/lookups'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) : LookupODataResponse => {
        const nextPage = result.data['@odata.nextLink'];
        return { data: result.data.value as Lookup[], nextPage }
    }

    async PostLookupAsync(lookup: Lookup) {
        return await this.apiBroker.PostAsync(this.relativeLookupUrl, lookup)
            .then(result => result.data as Lookup);
    }

    async GetAllLookupsAsync(queryString: string) {
        const url = this.relativeLookupUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as Lookup[]);
    }

    async GetLookupFirstPagesAsync(query: string) {
        const url = this.relativeLookupOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetLookupSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetLookupByIdAsync(id: string) {
        const url = `${this.relativeLookupUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as Lookup);
    }

    async PutLookupAsync(lookup: Lookup) {
        return await this.apiBroker.PutAsync(this.relativeLookupUrl, lookup)
            .then(result => result.data as Lookup);
    }

    async DeleteLookupByIdAsync(id: string) {
        const url = `${this.relativeLookupUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => result.data as Lookup);
    }
}
export default LookupBroker;