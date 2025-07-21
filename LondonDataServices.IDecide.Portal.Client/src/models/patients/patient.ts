export class Patient {
    public id: string;
    public nhsNumber?: string;
    public firstName?: string;
    public surname?: string;
    public emailAddress?: string;
    public address?: string;
    public postcode?: string;
    public dateOfBirth?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(patient: Patient) {
        this.id = patient.id || "";
        this.nhsNumber = patient.nhsNumber || "";
        this.firstName = patient.firstName || "";
        this.surname = patient.surname || "";
        this.emailAddress = patient.emailAddress || "";
        this.address = patient.address || "";
        this.postcode = patient.postcode || "";
        this.dateOfBirth = patient.dateOfBirth ? new Date(patient.dateOfBirth) : undefined;
        this.createdBy = patient.createdBy || "";
        this.createdDate = patient.createdDate ? new Date(patient.createdDate) : undefined;
        this.updatedBy = patient.updatedBy || "";
        this.updatedDate = patient.updatedDate ? new Date(patient.updatedDate) : undefined;
    }
}