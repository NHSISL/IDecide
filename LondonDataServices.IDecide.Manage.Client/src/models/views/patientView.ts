export class PatientView {
    public id: string;
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
    public validationCodeMatchedOn?: Date;
    public retryCount?: number;
    public notificationPreference?: number;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        nhsNumber?: string,
        title?: string,
        givenName?: string,
        surname?: string,
        dateOfBirth?: Date,
        gender?: string,
        email?: string,
        phone?: string,
        address?: string,
        postCode?: string,
        validationCode?: string,
        validationCodeExpiresOn?: Date,
        validationCodeMatchedOn?: Date,
        retryCount?: number,
        notificationPreference?: number,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date
    ) {
        this.id = id;
        this.nhsNumber = nhsNumber;
        this.title = title;
        this.givenName = givenName;
        this.surname = surname;
        this.dateOfBirth = dateOfBirth;
        this.gender = gender;
        this.email = email;
        this.phone = phone;
        this.address = address;
        this.postCode = postCode;
        this.validationCode = validationCode;
        this.validationCodeExpiresOn = validationCodeExpiresOn;
        this.validationCodeMatchedOn = validationCodeMatchedOn;
        this.retryCount = retryCount;
        this.notificationPreference = notificationPreference;
        this.createdBy = createdBy;
        this.createdDate = createdDate;
        this.updatedBy = updatedBy;
        this.updatedDate = updatedDate;
    }
}