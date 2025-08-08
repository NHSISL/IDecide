import { Decision } from "../models/decisions/decision";
import ApiBroker from "./apiBroker";

class DecisionBroker {
    relativeDecisionsUrl = '/api/decisions';
    private apiBroker: ApiBroker = new ApiBroker();

    async PostDecisionAsync(decision: Decision) {
        const url = this.relativeDecisionsUrl;
        return await this.apiBroker.PostAsync(url, decision)
            .then(result => new Decision(result.data));
    }
}

export default DecisionBroker;