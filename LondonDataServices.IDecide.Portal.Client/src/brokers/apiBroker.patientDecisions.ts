import { Decision } from "../models/decisions/decision";
import ApiBroker from "./apiBroker";

class PatientDecisionBroker {
    relativeDecisionsUrl = '/api/PatientDecision';
    private apiBroker: ApiBroker = new ApiBroker();

    async PostPatientDecisionAsync(decision: Decision, headers?: Record<string, string>) {
        const url = `${this.relativeDecisionsUrl}/PatientDecision`;

            return await this.apiBroker.PostAsync(url, decision, headers)
                .then(() => undefined);
    }
}

export default PatientDecisionBroker;