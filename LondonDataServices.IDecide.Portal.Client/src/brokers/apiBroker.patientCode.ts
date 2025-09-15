import ApiBroker from "./apiBroker";
import { PatientCodeRequest } from "../models/patients/patientCodeRequest";

class PatientCodeBroker {
    relativePatientCodeUrl = '/api/PatientCode';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostPatientWithNotificationPreference(patient: PatientCodeRequest, headers?: Record<string, string>) {
        const url = `${this.relativePatientCodeUrl}/PatientGenerationRequest`;
        return await this.apiBroker.PostAsync(url, patient, headers)
            .then(() => undefined);
    }
}

export default PatientCodeBroker;