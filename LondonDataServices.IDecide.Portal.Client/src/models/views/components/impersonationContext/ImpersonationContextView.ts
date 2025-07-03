export class ImpersonationContextView {
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
    public organisation: string = "";
    public projectName: string = "";
    public inboxToken?: string;
    public outboxToken?: string;
    public errorToken?: string;
    public isApproved: boolean = false;
    public identifierColumn: string = "";
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}