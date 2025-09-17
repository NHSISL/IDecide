export class PatientCodeRequest {
    public nhsNumber: string;
    public verificationCode: string;
    public notificationPreference: string;
    public generateNewCode: boolean;

    constructor(patientCode: PatientCodeRequest) {
        this.nhsNumber = patientCode.nhsNumber || "";
        this.verificationCode = patientCode.verificationCode || "";
        this.notificationPreference = patientCode.notificationPreference || "";
        this.generateNewCode = patientCode.generateNewCode || false;
    }
}