export class UserAccess {
    public id: string = crypto.randomUUID();
    public displayName: string = "";
    public entraGuid: string = "";
    public userPrincipalName: string = "";
    public UserId: string = "";
    public email: string = "";
    public givenName: string = "TODO";
    public surname: string = "TODO";
    public orgCodes: string[] = [];
    public orgCode: string = "";
    public jobTitle: string = "TODO";
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}