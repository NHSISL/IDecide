import { Decision } from "../models/decisions/decision";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

type DecisionODataResponse = {
    data: Decision[],
    nextPage: string
}

class DecisionBroker {
    relativeDecisionsUrl = '/api/Decision';
    relativeDecisionsOdataUrl = '/odata/Decisions'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): DecisionODataResponse => {
        const nextPage = result.data['@odata.nextLink'];
        return { data: result.data.value as Decision[], nextPage }
    }

    async GetAllDecisionsAsync(queryString: string) {
        const url = this.relativeDecisionsUrl + queryString;

        if (queryString === "/") {
            return undefined;
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((item: Decision) => new Decision(item)));
    }

    async GetDecisionFirstPagesAsync(query: string) {
        const url = this.relativeDecisionsOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetDecisionSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async PostPatientDecisionAsync(decision: Decision) {
        const url = `${this.relativeDecisionsUrl}/PatientDecision`;

            return await this.apiBroker.PostAsync(url, decision)
                .then(() => undefined);
    }
}

export default DecisionBroker;