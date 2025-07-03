
export class PdsData {
    public id: string;
    public pseudoNhsNumber: string;
    public orgCode: string;
    public organisationName: string;
    public relationshipWithOrganisationEffectiveFromDate?: Date | undefined;
    public relationshipWithOrganisationEffectiveToDate?: Date | undefined;

    constructor(pds: PdsData) {
            this.id = pds.id;
            this.pseudoNhsNumber = pds.pseudoNhsNumber;
            this.orgCode = pds.orgCode;
            this.organisationName = pds.organisationName
            this.relationshipWithOrganisationEffectiveFromDate = pds.relationshipWithOrganisationEffectiveFromDate
            this.relationshipWithOrganisationEffectiveToDate = pds.relationshipWithOrganisationEffectiveToDate;
    }
}