import { PatientDecision } from "../models/patientDecisions/patientDecision";
import ApiBroker from "./apiBroker";

class PatientDecisionBroker {
    relativeDecisionsUrl = '/api/PatientDecision';
    private apiBroker: ApiBroker = new ApiBroker();

    async PostPatientDecisionAsync(decision: PatientDecision, headers?: Record<string, string>) {
        const url = `${this.relativeDecisionsUrl}/PatientDecision`;

            return await this.apiBroker.PostAsync(url, decision, headers)
                .then(() => undefined);
    }

    async PostPatientDecisionNhsLoginAsync(decision: PatientDecision) {
        const url = `${this.relativeDecisionsUrl}/PatientDecisionNhsLogin`;

        return await this.apiBroker.PostAsync(url, decision)
            .then(() => undefined);
    }
}

export default PatientDecisionBroker;