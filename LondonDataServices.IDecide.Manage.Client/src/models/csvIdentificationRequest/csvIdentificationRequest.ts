export class CsvIdentificationRequest {
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

    constructor(csvIdentificationRequest: CsvIdentificationRequest) {
        this.id = csvIdentificationRequest.id ? csvIdentificationRequest.id : "";
        this.requesterFirstName = csvIdentificationRequest.requesterFirstName || "";
        this.requesterLastName = csvIdentificationRequest.requesterLastName || "";
        this.requesterEmail = csvIdentificationRequest.requesterEmail || "";
        this.recipientFirstName = csvIdentificationRequest.recipientFirstName || "";
        this.recipientLastName = csvIdentificationRequest.recipientLastName || "";
        this.reason = csvIdentificationRequest.reason || "";
        this.purpose = csvIdentificationRequest.purpose || "";
        this.organisation = csvIdentificationRequest.organisation || "";
        this.data = csvIdentificationRequest.data || 0;
        this.sha256Hash = csvIdentificationRequest.sha256Hash || "";
        this.identifierColumn = csvIdentificationRequest.identifierColumn || ""; 
        this.filepath = csvIdentificationRequest.filepath || ""; 
        this.createdDate = csvIdentificationRequest.createdDate ? new Date(csvIdentificationRequest.createdDate) : undefined;
        this.createdBy = csvIdentificationRequest.createdBy || "";
        this.updatedDate = csvIdentificationRequest.updatedDate ? new Date(csvIdentificationRequest.updatedDate) : undefined;
        this.updatedBy = csvIdentificationRequest.updatedBy || "";
    }
}