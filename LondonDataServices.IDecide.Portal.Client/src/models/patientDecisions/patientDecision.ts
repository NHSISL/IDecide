import { Patient } from "../patients/patient";
import { DecisionType } from "../decisionTypes/decisionType";
import { ConsumerAdoption } from "../consumerAdoptions/consumerAdoption";

export class PatientDecision {
    public id?: string;
    public patientId?: string;
    public decisionTypeId?: string;
    public decisionChoice?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public responsiblePersonGivenName?: string;
    public responsiblePersonSurname?: string;
    public responsiblePersonRelationship?: string;
    public decisionType?: DecisionType;
    public patient?: Patient;
    public consumerAdoptions?: ConsumerAdoption[];

    constructor(decision?: Partial<PatientDecision>) {
        this.id = decision?.id || '';
        this.patientId = decision?.patientId || '';
        this.decisionTypeId = decision?.decisionTypeId || '';
        this.decisionChoice = decision?.decisionChoice || '';
        this.createdBy = decision?.createdBy || '';
        this.createdDate = decision?.createdDate ? new Date(decision.createdDate) : undefined;
        this.updatedBy = decision?.updatedBy || '';
        this.updatedDate = decision?.updatedDate ? new Date(decision.updatedDate) : undefined;
        this.responsiblePersonGivenName = decision?.responsiblePersonGivenName || '';
        this.responsiblePersonSurname = decision?.responsiblePersonSurname || '';
        this.responsiblePersonRelationship = decision?.responsiblePersonRelationship || '';
        this.decisionType = decision?.decisionType;
        this.patient = decision?.patient;
        this.consumerAdoptions = decision?.consumerAdoptions || [];
    }
}