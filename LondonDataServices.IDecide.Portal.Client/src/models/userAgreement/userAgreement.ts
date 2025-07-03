export class UserAgreement {
    public id: string = "";
    public UserId: string = "";
    public agreementVersion: string = "";
    public agreementType: string = "";
    public agreementDate?: Date | undefined;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}