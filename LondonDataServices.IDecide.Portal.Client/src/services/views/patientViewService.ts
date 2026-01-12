import { patientService } from "../foundations/patientService";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { PatientLookup } from "../../models/patients/patientLookup";
import { Patient } from "../../models/patients/patient";

export const patientViewService = {
    usePostPatientSearch: () => {
        return {
            mutate: async (
                patientLookup: PatientLookup,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: (createdPatient: Patient) => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    const createdPatient = await patientService.useCreatePatient(
                        patientLookup,
                        options?.headers
                    );
                    options?.onSuccess?.(createdPatient);
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },
    usePostPatientDetails: () => {
        return {
            mutate: async (
                patientLookup: PatientLookup,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: (createdPatient: Patient) => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    const createdPatient = await patientService.useCreatePatient(
                        patientLookup,
                        options?.headers
                    );
                    options?.onSuccess?.(createdPatient);
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },
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
    useAddPatientNhsLogin: () => {
        return {
            mutate: async (
                phoneNumber: string,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await patientService
                        .usePostPatientNhsLogin(phoneNumber, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },
    useConfirmCode: () => {
        return {
            mutate: async (
                request: PatientCodeRequest,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await patientService.useConfirmCode(request, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                    throw error;
                }
            }
        };
    },
    useRetrievePatientInfoNhsLogin: () => {
        return patientService.useRetrievePatientInfoNhsLogin();
    },
};