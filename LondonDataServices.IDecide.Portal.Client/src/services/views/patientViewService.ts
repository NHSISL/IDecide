import { patientService } from "../foundations/patientService";

export const patientViewService = {
    usePostPatientSearch: () => {
        return patientService.useCreatePatientByNhsNumber();
    },
    //usePostPatientDetails: () => {
    //    return patientService.useCreatePatientByDetails();
    //},
    useUpdatePatient: () => {
        return patientService.useGenerateCodeRequest();
    },
    useConfirmCode: () => {
        return patientService.useConfirmCode();
    },
};