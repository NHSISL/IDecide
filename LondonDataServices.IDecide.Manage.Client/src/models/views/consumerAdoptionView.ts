import { Consumer } from "../consumers/consumer";
import { PatientDecision } from "../patientDecisions/patientDecision";
export class ConsumerAdoptionView {
    public id?: string;
    public consumerId?: string;
    public decisionId?: string;
    public adoptionDate?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public consumer?: Consumer;
    public decision?: PatientDecision;

    constructor(consumerAdoption?: Partial<ConsumerAdoptionView>) {
        this.id = consumerAdoption?.id || '';
        this.consumerId = consumerAdoption?.consumerId || '';
        this.decisionId = consumerAdoption?.decisionId || '';
        this.adoptionDate = consumerAdoption?.adoptionDate ? new Date(consumerAdoption.adoptionDate) : undefined;
        this.createdBy = consumerAdoption?.createdBy || '';
        this.createdDate = consumerAdoption?.createdDate ? new Date(consumerAdoption.createdDate) : undefined;
        this.updatedBy = consumerAdoption?.updatedBy || '';
        this.updatedDate = consumerAdoption?.updatedDate ? new Date(consumerAdoption.updatedDate) : undefined;
        this.consumer = consumerAdoption?.consumer;
        this.decision = consumerAdoption?.decision;
    }
}