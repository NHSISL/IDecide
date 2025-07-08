import { OdsData } from "../models/odsData/odsData";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

type OdsODataResponse = {
    data: OdsData[],
    nextPage: string
}

class OdsDataBroker {
    relativeOdsDataUrl = '/api/odsData';
    relativeOdsDataOdataUrl = '/odata/odsData'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): OdsODataResponse => {
        const data = result.data.value.map((odsData: OdsData) => odsData as OdsData);

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage };
    }

    async GetAllOdsDataAsync(queryString: string) {
        const url = this.relativeOdsDataUrl + queryString;

        if(!queryString) {
            return []
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as OdsData[]);
    }

    async GetOdsChildrenByIdAsync(id: string) {
        const url = `${this.relativeOdsDataUrl}/GetChildren/${id}`;
        
        if(!id) {
            return [];
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as OdsData[]);
    }

    async GetOdsDataFirstPagesAsync(query: string) {
        const url = this.relativeOdsDataOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetOdsDataSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }
}
export default OdsDataBroker;