
export class User {
    public id: string = "";
    public displayName: string = "";
    public givenName: string = "";
    public surname: string = "";
    public jobTitle: string = "";
    public mail: string = "";
    public userPrincipalName: string = "";
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}