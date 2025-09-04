import { patientService } from "../foundations/patientService";
import { GenerateCodeRequest } from "../../models/patients/generateCodeRequest";

export const patientViewService = {
    usePostPatientSearch: () => {
        return patientService.useCreatePatientByNhsNumber();
    },
    usePostPatientDetails: () => {
        return patientService.useCreatePatientByDetails();
    },
    useAddPatient: () => {
        return {
            mutate: async (
                patient: GenerateCodeRequest,
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