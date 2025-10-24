import { Consumer } from "../models/consumers/consumer";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

type ConsumerODataResponse = {
    data: Consumer[],
    nextPage: string
}

class ConsumerBroker {
    relativeConsumersUrl = '/api/Consumers';
    relativeConsumersOdataUrl = '/odata/Consumers'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): ConsumerODataResponse => {
        const nextPage = result.data['@odata.nextLink'];
        return { data: result.data.value as Consumer[], nextPage }
    }

    async PostConsumerAsync(consumer: Consumer) {
        return await this.apiBroker.PostAsync(this.relativeConsumersUrl, consumer)
            .then(result => result.data as Consumer);
    }

    async GetAllConsumersAsync(queryString: string) {
        const url = this.relativeConsumersUrl + queryString;

        if (queryString === "/") {
            return undefined;
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((item: Consumer) => new Consumer(item)));
    }

    async GetConsumerFirstPagesAsync(query: string) {
        const url = this.relativeConsumersOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetConsumerSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async PostPatientConsumerAsync(consumer: Consumer) {
        const url = `${this.relativeConsumersUrl}/Consumer`;

        return await this.apiBroker.PostAsync(url, consumer)
                .then(() => undefined);
    }

    async PutConsumerAsync(consumer: Consumer) {
        return await this.apiBroker.PutAsync(this.relativeConsumersUrl, consumer)
            .then(result => result.data as Consumer);
    }

    async DeleteConsumerByIdAsync(id: string) {
        const url = `${this.relativeConsumersUrl}/${id}`;
        return await this.apiBroker.DeleteAsync(url)
            .then(result => result.data as Consumer);
    }
}

export default ConsumerBroker;