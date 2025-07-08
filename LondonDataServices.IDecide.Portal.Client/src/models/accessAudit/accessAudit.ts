export class AccessAudit {
    public id: string = "";
    public requestId: string = "";
    public pseudoIdentifier: string = "";
    public UserId: string = "";
    public givenName: string = "";
    public surname: string = "";
    public email: string = "";
    public reason: string = "";
    public organisation: string = "";
    public hasAccess: boolean = false;
    public message: string = "";
    public auditType: string = "";
    public transactionId: string = "";
    public count: number = 0
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}