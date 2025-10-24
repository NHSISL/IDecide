import { ConsumerAdoption } from "../consumerAdoptions/consumerAdoption";
export class ConsumerView {
    public id?: string;
    public entraId?: string;
    public name?: string;
    public contactPerson?: string;
    public contactNumber?: string;
    public contactEmail?: string;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;
    public consumerAdoptions?: ConsumerAdoption[];

    constructor(consumer?: Partial<ConsumerView>) {
        this.id = consumer?.id || '';
        this.entraId = consumer?.entraId || '';
        this.name = consumer?.name || '';
        this.contactPerson = consumer?.contactPerson || '';
        this.contactNumber = consumer?.contactNumber || '';
        this.contactEmail = consumer?.contactEmail || '';
        this.createdBy = consumer?.createdBy || '';
        this.createdDate = consumer?.createdDate ? new Date(consumer.createdDate) : undefined;
        this.updatedBy = consumer?.updatedBy || '';
        this.updatedDate = consumer?.updatedDate ? new Date(consumer.updatedDate) : undefined;
        this.consumerAdoptions = consumer?.consumerAdoptions || [];
    }
}