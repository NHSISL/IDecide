import { Patient } from "../models/patients/patient";
import { PatientLookup } from "../models/patients/patientLookup";
import ApiBroker from "./apiBroker";

class PatientSearchBroker {
    relativePatientsUrl = '/api/PatientSearch';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostPatientNhsNumberAsync(patientLookup: PatientLookup, headers?: Record<string, string>) {
        const url = `${this.relativePatientsUrl}/PatientSearch`;
        return await this.apiBroker.PostAsync(url, patientLookup, headers)
            .then(result => new Patient(result.data));
    }
}

export default PatientSearchBroker;