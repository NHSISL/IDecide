export class CsvIdentificationRequest {
    public id?: string = "";
    public requesterUserId?: string = "";
    public requesterFirstName?: string = "";
    public requesterLastName?: string = "";
    public requesterDisplayName?: string = "";
    public requesterEmail?: string = "";
    public requesterJobTitle?: string = "";
    public recipientUserId?: string = "";
    public recipientFirstName?: string = "";
    public recipientLastName?: string = "";
    public recipientDisplayName?: string = "";
    public recipientEmail?: string = "";
    public recipientJobTitle?: string = "";
    public reason?: string = "";
    public organisation?: string = "";
    public data?: string = "";
    public sha256Hash?: string = "";
    public identifierColumn?: string = "";
    public filepath?: string = "";
    public hasHeaderRecord?: boolean = false;
    public identifierColumnIndex?: number = 0;
    public createdBy?: string = "";
    public createdDate?: Date | undefined;
    public updatedBy?: string = "";
    public updatedDate?: Date | undefined;

    constructor(data: Partial<CsvIdentificationRequest>) {
        Object.assign(this, data);
    }
}