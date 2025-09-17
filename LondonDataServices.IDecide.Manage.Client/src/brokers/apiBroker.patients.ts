import { Patient } from "../models/patients/patient";
import { GenerateCodeRequest } from "../models/patients/generateCodeRequest";
import { PatientLookup } from "../models/patients/patientLookup";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";

type PatientODataResponse = {
    data: Patient[],
    nextPage: string
}

class PatientBroker {
    relativePatientsUrl = '/api/Patients';
    relativePatientsOdataUrl = '/odata/Patients'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): PatientODataResponse => {
        const nextPage = result.data['@odata.nextLink'];
        return { data: result.data.value as Patient[], nextPage }
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

    async GetPatientFirstPagesAsync(query: string) {
        const url = this.relativePatientsOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetPatientSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetPatientByIdAsync(nhsNumber: string) {
        const url = `${this.relativePatientsUrl}/${nhsNumber}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new Patient(result.data));
    }

    async PutGenerateCodeRequestAsync(patient: GenerateCodeRequest) {
        const url = this.relativePatientsUrl; // No nhsNumber in the URL
        return await this.apiBroker.PutAsync(url, patient)
            .then(() => undefined); // No response body expected
    }

   
}

export default PatientBroker;