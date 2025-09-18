import { Decision } from '../decisions/decision';
import { Consumer } from '../consumers/consumer';

export class ConsumerAdoption {
    public id?: string;
    public consumerId?: string;
    public decisionId?: string;
    public adoptionDate?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public consumer?: Consumer;
    public decision?: Decision;

    constructor(adoption?: Partial<ConsumerAdoption>) {
        this.id = adoption?.id || '';
        this.consumerId = adoption?.consumerId || '';
        this.decisionId = adoption?.decisionId || '';
        this.adoptionDate = adoption?.adoptionDate ? new Date(adoption.adoptionDate) : undefined;
        this.createdBy = adoption?.createdBy || '';
        this.createdDate = adoption?.createdDate ? new Date(adoption.createdDate) : undefined;
        this.updatedBy = adoption?.updatedBy || '';
        this.updatedDate = adoption?.updatedDate ? new Date(adoption.updatedDate) : undefined;
        this.consumer = adoption?.consumer;
        this.decision = adoption?.decision;
    }
}