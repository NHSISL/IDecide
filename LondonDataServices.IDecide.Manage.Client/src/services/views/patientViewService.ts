import { patientService } from "../foundations/patientService";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";

export const patientViewService = {
    usePostPatientSearch: () => {
        return patientService.useCreatePatient();
    },

    //usePostPatientDetails: () => {
    //    return {
    //        mutate: async (
    //            patientLookup: PatientLookup,
    //            options?: {
    //                headers?: Record<string, string>,
    //                onSuccess?: (createdPatient: Patient) => void,
    //                onError?: (error: unknown) => void
    //            }
    //        ) => {
    //            try {
    //                const createdPatient = await patientService.useCreatePatient(
    //                    patientLookup
    //                );
    //                options?.onSuccess?.(createdPatient);
    //            } catch (error) {
    //                options?.onError?.(error);
    //            }
    //        }
    //    };
    //},
    useAddPatientAndGenerateCode: () => {
        return patientService.usePostPatientWithNotificationPreference();
    },
    useConfirmCode: () => {
        return patientService.useConfirmCode();
    },
};