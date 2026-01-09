import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { patientDecisionService } from "../foundations/patientDecisionService";

export const decisionViewService = {
    useCreatePatientDecision: () => {
        return {
            mutate: async (
                decision: PatientDecision,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await patientDecisionService
                        .useCreatePatientDecision(decision, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },
    useCreatePatientDecisionNhsLogin: () => {
        return {
            mutate: async (
                decision: PatientDecision,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await patientDecisionService
                        .useCreatePatientDecisionNhsLogin(decision, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },

    useUpdatePatientDecision: () => {
        return patientDecisionService.useModifyPatientDecision();
    },
};