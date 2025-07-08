export class CsvIdentificationRequestView {
    public id: string;
    public requesterFirstName: string;
    public requesterLastName: string;
    public requesterEmail: string;
    public recipientFirstName: string;
    public recipientLastName: string;
    public reason: string;
    public purpose: string;
    public organisation: string;
    public data: Uint8Array;
    public sha256Hash: string;
    public identifierColumn: string;
    public filepath: string;
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
        reason: string,
        purpose: string,
        organisation: string,
        data: Uint8Array,
        sha256Hash: string,
        identifierColumn: string,
        filepath: string,
        createdBy?: string,
        createdDate?: Date | undefined,
        updatedBy?: string,
        updatedDate?: Date | undefined
    ) {
        this.id = id;
        this.requesterFirstName = requesterFirstName || "";
        this.requesterLastName = requesterLastName || "";
        this.requesterEmail = requesterEmail || "";
        this.recipientFirstName = recipientFirstName || "";
        this.recipientLastName = recipientLastName || "";
        this.reason = reason || "";
        this.purpose = purpose || "";
        this.organisation = organisation || "";
        this.data = data || 0;
        this.sha256Hash = sha256Hash || "";
        this.identifierColumn = identifierColumn || "";
        this.filepath = filepath || "";
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate ;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}