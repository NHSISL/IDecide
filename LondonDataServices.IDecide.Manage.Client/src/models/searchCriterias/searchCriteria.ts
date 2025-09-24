export class SearchCriteria {
    public nhsNumber: string = "";
    public firstName: string = "";
    public surname: string = "";
    public gender: string = "";
    public postcode: string = "";
    public dateOfBirth: string = "";
    public dateOfDeath: string = "";
    public registeredGpPractice: string = "";
    public email: string = "";
    public phoneNumber: string = "";

    constructor(searchCriteria?: Partial<SearchCriteria>) {
        if (searchCriteria) {
            this.nhsNumber = searchCriteria.nhsNumber ?? "";
            this.firstName = searchCriteria.firstName ?? "";
            this.surname = searchCriteria.surname ?? "";
            this.gender = searchCriteria.gender ?? "";
            this.postcode = searchCriteria.postcode ?? "";
            this.dateOfBirth = searchCriteria.dateOfBirth ?? "";
            this.dateOfDeath = searchCriteria.dateOfDeath ?? "";
            this.registeredGpPractice = searchCriteria.registeredGpPractice ?? "";
            this.email = searchCriteria.email ?? "";
            this.phoneNumber = searchCriteria.phoneNumber ?? "";
        }
    }
}