import DecisionBroker from "../../brokers/apiBroker.decisions";
import { useQueryClient, useMutation } from "@tanstack/react-query";
import { Decision } from "../../models/decisions/decision";

export const decisionService = {
    useCreateDecision: async(
        decison: Decision,
        headers?: Record<string, string>
    ) => {
        const broker = new DecisionBroker();

        return await broker.PostDecisionAsync(decison, headers);
    },

    useModifyDecision: () => {
        const broker = new DecisionBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (decision: Decision) => {
                const date = new Date();
                decision.updatedDate = date;

                return broker.PostDecisionAsync(decision);
            },
            onSuccess: () => {
                queryClient.invalidateQueries({ queryKey: ["DecisionGetAll"] });
            }
        });
    },
};