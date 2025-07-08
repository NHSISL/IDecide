import ApiBroker from "./apiBroker";
import { Lookup } from "../models/lookups/lookup";

class LookupBroker {
    relativeLookupUrl = '/api/lookups';
    relativeLookupOdataUrl = '/odata/lookups'

    private apiBroker: ApiBroker = new ApiBroker();

    async GetAllLookupsAsync(queryString: string) {
        const url = this.relativeLookupUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as Lookup[]);
    }
}
export default LookupBroker;