export class UserAccessView {
    public id: string;
    public email: string;
    public displayName: string;
    public UserId: string;
    public entraUpn: string;
    public givenName: string ;
    public surname: string;
    public userPrincipalName: string;
    public jobTitle: string;
    public orgCode: string;
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        email: string,
        displayName: string,
        UserId: string,
        entraUpn: string,
        givenName: string,
        surname: string,
        userPrincipalName: string,
        jobTitle: string,
        orgCode: string,
        activeFrom?: Date,
        activeTo?: Date,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date,
    ) {
        this.id = id;
        this.email = email || "";
        this.orgCode = orgCode || "";
        this.displayName = displayName;
        this.UserId = UserId;
        this.entraUpn = entraUpn;
        this.givenName = givenName;
        this.surname = surname;
        this.userPrincipalName = userPrincipalName;
        this.jobTitle = jobTitle;
        this.activeFrom = activeFrom;
        this.activeTo = activeTo;
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate ;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}