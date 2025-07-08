export class ImpersonationContextView {
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

    constructor(
        id: string,
        requesterFirstName: string,
        requesterLastName: string,
        requesterEmail: string,
        recipientFirstName: string,
        recipientLastName: string,
        recipientEmail: string,
        reason: string,
        purpose: string,
        organisation: string,
        isApproved: boolean,
        identifierColumn: string,
        createdBy?: string,
        createdDate?: Date | undefined,
        updatedBy?: string,
        updatedDate?: Date | undefined,
    ) {
        this.id = id;
        this.requesterFirstName = requesterFirstName || "";
        this.requesterLastName = requesterLastName || "";
        this.requesterEmail = requesterEmail || "";
        this.recipientFirstName = recipientFirstName || "";
        this.recipientLastName = recipientLastName || "";
        this.recipientEmail = recipientEmail || "";
        this.reason = reason || "";
        this.purpose = purpose || "";
        this.organisation = organisation || "";
        this.isApproved = isApproved || false;
        this.identifierColumn = identifierColumn || "";
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate ;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}