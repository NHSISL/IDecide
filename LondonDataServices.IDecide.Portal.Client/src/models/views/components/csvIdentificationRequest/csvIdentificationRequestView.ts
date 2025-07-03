export class CsvIdentificationRequestView {
    public id?: string;
    public requesterUserId?: string;
    public requesterFirstName?: string;
    public requesterLastName?: string;
    public requesterDisplayName?: string;
    public requesterEmail?: string;
    public recipientUserId?: string;
    public recipientFirstName?: string;
    public recipientLastName?: string;
    public recipientDisplayName?: string;
    public recipientEmail?: string;
    public reason?: string;
    public organisation?: string;
    public hasHeaderRecord?: boolean;
    public identifierColumnIndex?: number;
    public createdDate?: Date;
}