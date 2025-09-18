import { Decision } from "../../models/decisions/decision";
import { decisionService } from "../foundations/decisionService";

export const decisionViewService = {
    useCreateDecision: () => {
        return {
            mutate: async (
                decision: Decision,
                options?: {
                    headers?: Record<string, string>,
                    onSuccess?: () => void,
                    onError?: (error: unknown) => void
                }
            ) => {
                try {
                    await decisionService
                        .useCreateDecision(decision, options?.headers);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },

    useUpdateDecision: () => {
        return decisionService.useModifyDecision();
    },
};