export class UserAccess {
    public id: string = "";
    public displayName: string = "";
    public UserId: string = "";
    public entraUpn: string = "";
    public givenName: string = "";
    public surname: string = "";
    public email: string = "";
    public userPrincipalName: string = "";
    public orgCode: string = "";
    public jobTitle: string = "";
    public activeFrom?: Date | undefined;
    public activeTo?: Date | undefined;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}