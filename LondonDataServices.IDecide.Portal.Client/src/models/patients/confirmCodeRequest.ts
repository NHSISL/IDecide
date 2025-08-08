export class ConfirmCodeRequest {
    public nhsNumber?: string;
    public code?: string;
    constructor(patient: ConfirmCodeRequest) {
        this.nhsNumber = patient.nhsNumber || "";
        this.code = patient.code || "";
    }
}