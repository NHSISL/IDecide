import { patientService } from "../foundations/patientService";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";

export const patientViewService = {
    usePostPatientSearch: () => {
        return patientService.useCreatePatientByNhsNumber();
    },
    //usePostPatientDetails: () => {
    //    return patientService.useCreatePatientByDetails();
    //},

    useAddPatientAndGenerateCode: () => {
        return {
            mutate: async (
                patient: PatientCodeRequest,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await patientService
                        .usePostPatientWithNotificationPreference(patient, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },
    useConfirmCode: () => {
        return patientService.useConfirmCode();
    },
};