import { Decision } from '../decisions/decision';

export class DecisionType {
    public id?: string;
    public name?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public decisions?: Decision[];

    constructor(type?: Partial<DecisionType>) {
        this.id = type?.id || '';
        this.name = type?.name || '';
        this.createdBy = type?.createdBy || '';
        this.createdDate = type?.createdDate ? new Date(type.createdDate) : undefined;
        this.updatedBy = type?.updatedBy || '';
        this.updatedDate = type?.updatedDate ? new Date(type.updatedDate) : undefined;
        this.decisions = type?.decisions || [];
    }
}