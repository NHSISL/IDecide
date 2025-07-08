export class ImpersonationContext {
    public id: string;
    public requesterFirstName: string;
    public requesterLastName: string;
    public requesterEmail: string;
    public recipientFirstName: string;
    public recipientLastName: string;
    public recipientEmail: string;
    public reason: string;
    public purpose: string;
    public organisation: string;
    public isApproved: boolean;
    public identifierColumn: string;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(impersonationContext: ImpersonationContext) {
        this.id = impersonationContext.id ? impersonationContext.id : "";
        this.requesterFirstName = impersonationContext.requesterFirstName || "";
        this.requesterLastName = impersonationContext.requesterLastName || "";
        this.requesterEmail = impersonationContext.requesterEmail || "";
        this.recipientFirstName = impersonationContext.recipientFirstName || "";
        this.recipientLastName = impersonationContext.recipientLastName || "";
        this.recipientEmail = impersonationContext.recipientEmail || "";
        this.reason = impersonationContext.reason || "";
        this.purpose = impersonationContext.purpose || "";
        this.organisation = impersonationContext.organisation || "";
        this.isApproved = impersonationContext.isApproved || false;
        this.identifierColumn = impersonationContext.identifierColumn || ""; 
        this.createdDate = impersonationContext.createdDate ? new Date(impersonationContext.createdDate) : undefined;
        this.createdBy = impersonationContext.createdBy || "";
        this.updatedDate = impersonationContext.updatedDate ? new Date(impersonationContext.updatedDate) : undefined;
        this.updatedBy = impersonationContext.updatedBy || "";
    }
}