import { ConsumerAdoption } from '../consumerAdoptions/consumerAdoption';

export class Consumer {
    public id?: string;
    public name?: string;
    public accessToken?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public consumerAdoptions?: ConsumerAdoption[];

    constructor(consumer?: Partial<Consumer>) {
        this.id = consumer?.id || '';
        this.name = consumer?.name || '';
        this.accessToken = consumer?.accessToken || '';
        this.createdBy = consumer?.createdBy || '';
        this.createdDate = consumer?.createdDate ? new Date(consumer.createdDate) : undefined;
        this.updatedBy = consumer?.updatedBy || '';
        this.updatedDate = consumer?.updatedDate ? new Date(consumer.updatedDate) : undefined;
        this.consumerAdoptions = consumer?.consumerAdoptions || [];
    }
}