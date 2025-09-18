import { Decision } from "../models/decisions/decision";
import ApiBroker from "./apiBroker";

class DecisionBroker {
    relativeDecisionsUrl = '/api/PatientDecision';
    private apiBroker: ApiBroker = new ApiBroker();

    async PostDecisionAsync(decision: Decision, headers?: Record<string, string>) {
        const url = `${this.relativeDecisionsUrl}/PatientDecision`;

            return await this.apiBroker.PostAsync(url, decision, headers)
                .then(() => undefined);
    }
}

export default DecisionBroker;