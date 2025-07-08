export class OdsData {
    public id: string = crypto.randomUUID();
    public organisationCode: string = "";
    public organisationName: string = "";
    public odsHierarchy: string = "";
    public relationshipStartDate?: Date;
    public relationshipEndDate?: Date;
    public hasChildren: boolean = false;
}