import { Decision } from "../models/decisions/decision";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

class DecisionBroker {
    relativeDecisionsUrl = '/api/decisions';
    relativeDecisionsOdataUrl = '/odata/decisions'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((decision: Decision) => new Decision(decision));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostDecisionAsync(decision: Decision) {
        const url = `${this.relativeDecisionsUrl}/CreateDecision`;
        return await this.apiBroker.PostAsync(url, decision)
            .then(result => new Decision(result.data));
    }

    async GetAllDecisionsAsync(queryString: string) {
        const url = this.relativeDecisionsUrl + queryString;

        if (queryString === "/") {
            return undefined;
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((item: Decision) => new Decision(item)));
    }

    async GetDecisionsOdataAsync(query: string) {
        const url = this.relativeDecisionsOdataUrl + (query || "");
        const response = await this.apiBroker.GetAsync(url);
        return this.processOdataResult(response);
    }

    async GetDecisionByIdAsync(decisionId: string) {
        const url = `${this.relativeDecisionsUrl}/${decisionId}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new Decision(result.data));
    }

    async PutDecisionAsync(decision: Decision) {
        const url = `${this.relativeDecisionsUrl}/${decision.id}`;
        return await this.apiBroker.PutAsync(url, decision)
            .then(result => new Decision(result.data));
    }
}

export default DecisionBroker;