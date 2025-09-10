export class PatientCodeRequest {
    public nhsNumber: string;
    public verificationCode: string;
    public notificationPreference: string;
    public generateNewCode: boolean;

    constructor(patient: PatientCodeRequest) {
        this.nhsNumber = patient.nhsNumber || "";
        this.verificationCode = patient.verificationCode || "";
        this.notificationPreference = patient.notificationPreference || "";
        this.generateNewCode = patient.generateNewCode || false;
    }
}