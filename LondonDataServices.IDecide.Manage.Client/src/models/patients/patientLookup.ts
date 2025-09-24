import { SearchCriteria } from "../searchCriterias/searchCriteria";
import { Patient } from "./patient";

export class PatientLookup {
    public searchCriteria: SearchCriteria;
    public patients: Patient[];

    constructor(searchCriteria?: Partial<SearchCriteria>, patients?: Patient[]) {
        this.searchCriteria = new SearchCriteria(searchCriteria ?? {});
        this.patients = patients ?? [];
    }
}