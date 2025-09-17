export class Decision {
    public patientNhsNumber?: string;
    public decisionTypeId?: number;
    public decisionChoice?: string;
    public responsiblePersonGivenName?: string;
    public ResponsiblePersonSurname?: string;
    public ResponsiblePersonRelationship?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(decision: Decision) {
        this.patientNhsNumber = decision.patientNhsNumber || "";
        this.decisionTypeId = decision.decisionTypeId;
        this.decisionChoice = decision.decisionChoice || "";
        this.responsiblePersonGivenName = decision.responsiblePersonGivenName || "";
        this.ResponsiblePersonSurname = decision.ResponsiblePersonSurname || "";
        this.ResponsiblePersonRelationship = decision.ResponsiblePersonRelationship || "";
        this.createdBy = decision.createdBy || "";
        this.createdDate = decision.createdDate ? new Date(decision.createdDate) : undefined;
        this.updatedBy = decision.updatedBy || "";
        this.updatedDate = decision.updatedDate ? new Date(decision.updatedDate) : undefined;
    }
}