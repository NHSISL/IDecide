export class PatientView {
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

    constructor(
        id: string,
        nhsNumber?: string,
        firstName?: string,
        surname?: string,
        emailAddress?: string,
        address?: string,
        postcode?: string,
        dateOfBirth?: Date,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date
    ) {
        this.id = id;
        this.nhsNumber = nhsNumber || "";
        this.firstName = firstName || "";
        this.surname = surname || "";
        this.emailAddress = emailAddress || "";
        this.address = address || "";
        this.postcode = postcode || "";
        this.dateOfBirth = dateOfBirth;
        this.createdBy = createdBy || "";
        this.createdDate = createdDate;
        this.updatedBy = updatedBy;
        this.updatedDate = updatedDate;
    }
}