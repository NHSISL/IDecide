import { Patient } from "../models/patients/patient";
import { PatientLookup } from "../models/patients/patientLookup";
import ApiBroker from "./apiBroker";

class PatientSearchBroker {
    relativePatientsUrl = '/api/PatientSearch';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostPatientNhsNumberAsync(patientLookup: PatientLookup) {
        const url = `${this.relativePatientsUrl}/PatientSearch`;
        return await this.apiBroker.PostAsync(url, patientLookup)
            .then(result => new Patient(result.data));
    }

    //async PostPatientDetailsAsync(patientLookup: PatientLookup) {
    //    const url = `${this.relativePatientsUrl}/PostPatientByDetails`;
    //    return await this.apiBroker.PostAsync(url, patientLookup)
    //        .then(result => new Patient(result.data));
    //}
}

export default PatientSearchBroker;