
export class PdsDataView {
    public id: string;
    public pseudoNhsNumber: string;
    public orgCode: string;
    public organisationName: string;
    public relationshipWithOrganisationEffectiveFromDate?: Date | undefined;
    public relationshipWithOrganisationEffectiveToDate?: Date | undefined;

    constructor(
        id: string,
        pseudoNhsNumber: string,
        orgCode: string,
        organisationName: string,
        relationshipWithOrganisationEffectiveFromDate?: Date,
        relationshipWithOrganisationEffectiveToDate?: Date,
    ) {
        this.id = id;
        this.pseudoNhsNumber = pseudoNhsNumber;
        this.orgCode = orgCode;
        this.organisationName = organisationName
        this.relationshipWithOrganisationEffectiveFromDate = relationshipWithOrganisationEffectiveFromDate
        this.relationshipWithOrganisationEffectiveToDate = relationshipWithOrganisationEffectiveToDate;
    }
}