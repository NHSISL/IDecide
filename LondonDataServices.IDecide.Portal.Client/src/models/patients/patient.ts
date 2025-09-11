export class Patient {
    public id: string = "";
    public nhsNumber: string = "";
    public title: string = "";
    public givenName: string[] = []; // Changed from string to string[]
    public surname: string = "";
    public dateOfBirth?: Date;
    public gender: string = "";
    public email: string = "";
    public phone: string = "";
    public address: string = "";
    public postCode: string = "";
    public validationCode: string = "";
    public validationCodeExpiresOn?: Date;
    public retryCount: number = 0;
    public createdBy: string = "";
    public createdDate?: Date;
    public updatedBy: string = "";
    public updatedDate?: Date;

    constructor(patient?: Partial<Patient>) {
        if (patient) {
            this.id = patient.id ?? "";
            this.nhsNumber = patient.nhsNumber ?? "";
            this.title = patient.title ?? "";
            // Accepts string or string[] for compatibility
            if (Array.isArray(patient.givenName)) {
                this.givenName = patient.givenName;
            } else if (typeof patient.givenName === "string") {
                this.givenName = patient.givenName ? [patient.givenName] : [];
            } else {
                this.givenName = [];
            }
            this.surname = patient.surname ?? "";
            this.dateOfBirth = patient.dateOfBirth ? new Date(patient.dateOfBirth) : undefined;
            this.gender = patient.gender ?? "";
            this.email = patient.email ?? "";
            this.phone = patient.phone ?? "";
            this.address = patient.address ?? "";
            this.postCode = patient.postCode ?? "";
            this.validationCode = patient.validationCode ?? "";
            this.validationCodeExpiresOn = patient.validationCodeExpiresOn ? new Date(patient.validationCodeExpiresOn) : undefined;
            this.retryCount = patient.retryCount ?? 0;
            this.createdBy = patient.createdBy ?? "";
            this.createdDate = patient.createdDate ? new Date(patient.createdDate) : undefined;
            this.updatedBy = patient.updatedBy ?? "";
            this.updatedDate = patient.updatedDate ? new Date(patient.updatedDate) : undefined;
        }
    }
}