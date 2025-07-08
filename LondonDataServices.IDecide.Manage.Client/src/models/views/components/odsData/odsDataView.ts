export class OdsDataView {
    public id: string;
    public odsHierarchy: string;
    public organisationCode: string;
    public organisationName: string;
    public hasChildren: boolean;
    public relationshipWithParentStartDate?: Date | undefined;
    public relationshipWithParentEndDate?: Date | undefined;

    constructor(
        id: string,
        odsHierarchy: string,
        organisationCode: string,
        organisationName: string,
        hasChildren: boolean,
        relationshipWithParentStartDate?: Date,
        relationshipWithParentEndDate?: Date,
    ) {
        this.id = id;
        this.relationshipWithParentStartDate = relationshipWithParentStartDate;
        this.relationshipWithParentEndDate = relationshipWithParentEndDate;
        this.odsHierarchy = odsHierarchy;
        this.organisationCode = organisationCode;
        this.organisationName = organisationName;
        this.hasChildren = hasChildren;
    }
}