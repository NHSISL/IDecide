export class Patient {
    public id: string;
    public nhsNumber?: string;
    public firstName?: string;
    public surname?: string;
    public emailAddress?: string;
    public address?: string;
    public postcode?: string;
    public phoneNumber?: string;
    public verificationCode?: string;
    public notificationPreference?: string;
    public dateOfBirth?: Date;
    public recaptchaToken?: string;
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
        this.phoneNumber = patient.phoneNumber || "";
        this.verificationCode = patient.verificationCode || "";
        this.notificationPreference = patient.notificationPreference || "";
        this.dateOfBirth = patient.dateOfBirth ? new Date(patient.dateOfBirth) : undefined;
        this.recaptchaToken = patient.recaptchaToken || "";
        this.createdBy = patient.createdBy || "";
        this.createdDate = patient.createdDate ? new Date(patient.createdDate) : undefined;
        this.updatedBy = patient.updatedBy || "";
        this.updatedDate = patient.updatedDate ? new Date(patient.updatedDate) : undefined;
    }
}