export class DecisionView {
    public id: string;
    public patientNhsNumber?: string;
    public decisionTypeId?: number;
    public decisionChoice?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        patientNhsNumber?: string,
        decisionTypeId?: number,
        decisionChoice?: string,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date
    ) {
        this.id = id;
        this.patientNhsNumber = patientNhsNumber || "";
        this.decisionTypeId = decisionTypeId;
        this.decisionChoice = decisionChoice || "";
        this.createdBy = createdBy || "";
        this.createdDate = createdDate;
        this.updatedBy = updatedBy;
        this.updatedDate = updatedDate;
    }
}