import { decisionService } from "../foundations/decisionService";

export const decisionViewService = {
    useCreateDecision: () => {
        return decisionService.useCreateDecision();
    },
    useUpdateDecision: () => {
        return decisionService.useModifyDecision();
    },
};