export class GenerateCodeRequest {
    public nhsNumber?: string;
    public notificationPreference?: string;
    constructor(patient: GenerateCodeRequest) {
        this.nhsNumber = patient.nhsNumber || "";
        this.notificationPreference = patient.notificationPreference || "";
    }
}