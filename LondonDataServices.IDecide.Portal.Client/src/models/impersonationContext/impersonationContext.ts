export class ImpersonationContext {
    public id: string = "";
    public requesterUserId: string = "";
    public requesterFirstName: string = "";
    public requesterLastName: string = "";
    public requesterDisplayName: string = "";
    public requesterEmail: string = "";
    public requesterJobTitle: string = "";
    public responsiblePersonUserId: string = "";
    public responsiblePersonFirstName: string = "";
    public responsiblePersonLastName: string = "";
    public responsiblePersonDisplayName: string = "";
    public responsiblePersonEmail: string = "";
    public responsiblePersonJobTitle: string = "";
    public reason: string = "";
    public purpose?: string = "";
    public organisation: string = "";
    public projectName: string = "";
    public inboxSasToken?: string = "";
    public outboxSasToken?: string = "";
    public errorsSasToken?: string = "";
    public isApproved: boolean = false;
    public identifierColumn: string = "";
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}