export class Patient {
    public id?: string;
    public nhsNumber?: string;
    public title?: string;
    public givenName?: string;
    public surname?: string;
    public dateOfBirth?: Date;
    public gender?: string;
    public email?: string;
    public phone?: string;
    public address?: string;
    public postCode?: string;
    public validationCode?: string;
    public validationCodeExpiresOn?: Date;
    public retryCount?: number;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(patient?: Partial<Patient>) {
        if (patient) {
            this.id = patient.id;
            this.nhsNumber = patient.nhsNumber;
            this.title = patient.title;
            this.givenName = patient.givenName;
            this.surname = patient.surname;
            this.dateOfBirth = patient.dateOfBirth ? new Date(patient.dateOfBirth) : undefined;
            this.gender = patient.gender;
            this.email = patient.email;
            this.phone = patient.phone;
            this.address = patient.address;
            this.postCode = patient.postCode;
            this.validationCode = patient.validationCode;
            this.validationCodeExpiresOn = patient.validationCodeExpiresOn ? new Date(patient.validationCodeExpiresOn) : undefined;
            this.retryCount = patient.retryCount;
            this.createdBy = patient.createdBy;
            this.createdDate = patient.createdDate ? new Date(patient.createdDate) : undefined;
            this.updatedBy = patient.updatedBy;
            this.updatedDate = patient.updatedDate ? new Date(patient.updatedDate) : undefined;
        }
    }
}