import { Patient } from "../models/patients/patient";
import { GenerateCodeRequest } from "../models/patients/generateCodeRequest";
import { PatientLookup } from "../models/patients/patientLookup";
import ApiBroker from "./apiBroker";
import { AxiosResponse } from "axios";
import { PatientNhsLogin } from "../models/patients/patientNhsLogin";

class PatientBroker {
    relativePatientsUrl = '/api/Patient';
    relativePatientsOdataUrl = '/odata/Patient'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((patient: Patient) => new Patient(patient));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
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

    async PutGenerateCodeRequestAsync(patient: GenerateCodeRequest) {
        const url = this.relativePatientsUrl; // No nhsNumber in the URL
        return await this.apiBroker.PutAsync(url, patient)
            .then(() => undefined); // No response body expected
    }

    async GetPatientInfoNhsLoginAsync() {
        const response = await fetch('/api/Patients/patientInfo');

        const r = await response.json();

        if (!r) {
            return undefined;
        }

        return new PatientNhsLogin({
            nhsNumber: r.nhs_number,
            givenName: r.given_name,
            surname: r.family_name,
            dateOfBirth: r.birthdate ? new Date(r.birthdate) : undefined,
            email: r.email,
            phone: r.phone_number
        });
    }
}

export default PatientBroker;