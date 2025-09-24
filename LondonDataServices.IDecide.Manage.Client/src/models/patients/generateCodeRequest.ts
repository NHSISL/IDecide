export class GenerateCodeRequest {
    public nhsNumber?: string;
    public notificationPreference?: string;
    public poaFirstName?: string;
    public poaSurname?: string;
    public poaRelationship?: string;

    constructor(patient: GenerateCodeRequest) {
        this.nhsNumber = patient.nhsNumber || "";
        this.notificationPreference = patient.notificationPreference || "";
        this.poaFirstName = patient.poaFirstName || "";
        this.poaSurname = patient.poaSurname || "";
        this.poaRelationship = patient.poaRelationship || "";
    }
}