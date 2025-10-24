import { ConsumerAdoption } from "../models/consumerAdoptions/consumerAdoption";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

type ConsumerAdoptionODataResponse = {
    data: ConsumerAdoption[],
    nextPage: string
}

class ConsumerAdoptionBroker {
    relativeConsumerAdoptionsUrl = '/api/ConsumerAdoptions';
    relativeConsumerAdoptionsOdataUrl = '/odata/ConsumerAdoptions'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): ConsumerAdoptionODataResponse => {
        const nextPage = result.data['@odata.nextLink'];
        return { data: result.data.value as ConsumerAdoption[], nextPage }
    }

    async GetAllConsumerAdoptionsAsync(queryString: string) {
        const url = this.relativeConsumerAdoptionsUrl + queryString;

        if (queryString === "/") {
            return undefined;
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((item: ConsumerAdoption) => new ConsumerAdoption(item)));
    }

    async GetConsumerAdoptionFirstPagesAsync(query: string) {
        const url = this.relativeConsumerAdoptionsOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetConsumerAdoptionSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }
}

export default ConsumerAdoptionBroker;