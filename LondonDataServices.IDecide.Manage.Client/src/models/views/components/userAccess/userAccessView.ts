export class UserAccessView {
    public id: string;
    public displayName: string;
    public entraGuid: string;
    public userPrincipalName: string;
    public UserId: string;
    public email: string;
    public givenName: string;
    public surname: string;
    public orgCodes: string[];
    public orgCode: string;
    public jobTitle: string;
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        displayName: string,
        entraGuid: string,
        userPrincipalName: string,
        UserId: string,
        email: string,
        givenName: string,
        surname: string,
        orgCodes: string[],
        orgCode: string,
        jobTitle: string,
        activeFrom?: Date,
        activeTo?: Date,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date,
    ) {
        this.id = id;
        this.displayName = displayName;
        this.entraGuid = entraGuid;
        this.userPrincipalName = userPrincipalName;
        this.UserId = UserId;
        this.email = email;
        this.givenName = givenName;
        this.surname = surname;
        this.orgCodes = orgCodes || [];
        this.orgCode = orgCode;
        this.jobTitle = jobTitle;
        this.activeFrom = activeFrom;
        this.activeTo = activeTo;
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}