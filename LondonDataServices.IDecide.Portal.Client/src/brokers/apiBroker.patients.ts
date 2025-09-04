import { Patient } from "../models/patients/patient";
import { GenerateCodeRequest } from "../models/patients/generateCodeRequest";
import { PatientLookup } from "../models/patients/patientLookup";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

class PatientBroker {
    relativePatientsUrl = '/api/PatientSearch';
    relativePatientsOdataUrl = '/odata/PatientSearch'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((patient: Patient) => new Patient(patient));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostPatientNhsNumberAsync(patientLookup: PatientLookup) {
        const url = `${this.relativePatientsUrl}/PatientSearch`;
        return await this.apiBroker.PostAsync(url, patientLookup)
            .then(result => new Patient(result.data));
    }

    async PostPatientSearchByNhsNumberAsync(patientLookup: PatientLookup) {
        const url = `${this.relativePatientsUrl}/PatientByNhsNumber`;
        return await this.apiBroker.PostAsync(url, patientLookup)
            .then(result => new Patient(result.data));
    }

    async PostPatientWithNotificationPreference(patient: GenerateCodeRequest, headers?: Record<string, string>) {
        const url = `${this.relativePatientsUrl}/PatientGenerationRequest`;
        return await this.apiBroker.PostAsync(url, patient, headers)
            .then(() => undefined);
    }

    async PostPatientDetailsAsync(patientLookup: PatientLookup) {
        const url = `${this.relativePatientsUrl}/PostPatientByDetails`;
        return await this.apiBroker.PostAsync(url, patientLookup)
            .then(result => new Patient(result.data));
    }

    async GetAllPatientsAsync(queryString: string) {
        const url = this.relativePatientsUrl + queryString;

        if (queryString === "/") {
            return undefined;
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((item: Patient) => new Patient(item)));
    }

    async GetPatientsOdataAsync(query: string) {
        const url = this.relativePatientsOdataUrl + (query || "");
        const response = await this.apiBroker.GetAsync(url);
        return this.processOdataResult(response);
    }

    async GetPatientByIdAsync(nhsNumber: string) {
        const url = `${this.relativePatientsUrl}/${nhsNumber}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new Patient(result.data));
    }

   

    async ConfirmPatientCodeAsync(nhsNumber: string, code: string) {
        const url = `${this.relativePatientsUrl}/confirm-code`;
        const payload = { nhsNumber, code };
        return await this.apiBroker.PutAsync(url, payload)
            .then(result => result.data);
    }
}

export default PatientBroker;