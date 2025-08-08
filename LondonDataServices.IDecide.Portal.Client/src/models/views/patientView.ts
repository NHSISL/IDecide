export class PatientView {
    public id: string;
    public nhsNumber?: string;
    public firstName?: string;
    public surname?: string;
    public emailAddress?: string;
    public address?: string;
    public postcode?: string;
    public phoneNumber?: string;
    public dateOfBirth?: Date;
    public verificationCode?: string;
    public notificationPreference?: string;
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
        phoneNumber?: string,
        verificationCode?: string,
        notificationPreference?: string,
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
        this.phoneNumber = phoneNumber || "";
        this.dateOfBirth = dateOfBirth;
        this.verificationCode = verificationCode;
        this.notificationPreference = notificationPreference;
        this.createdBy = createdBy || "";
        this.createdDate = createdDate;
        this.updatedBy = updatedBy;
        this.updatedDate = updatedDate;
    }
}