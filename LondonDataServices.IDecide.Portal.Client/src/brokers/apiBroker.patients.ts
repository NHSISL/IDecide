import { Patient } from "../models/patients/patient";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

class PatientBroker {
    relativePatientsUrl = '/api/patients';
    relativePatientsOdataUrl = '/odata/patients'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((patient: Patient) => new Patient(patient));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostPatientAsync(patient: Patient) {
        return await this.apiBroker.PostAsync(this.relativePatientsUrl, patient)
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

    async PutPatientAsync(patient: Patient) {
        const url = `${this.relativePatientsUrl}/${patient.nhsNumber}`;
        return await this.apiBroker.PutAsync(url, patient)
            .then(result => new Patient(result.data));
    }
}

export default PatientBroker;