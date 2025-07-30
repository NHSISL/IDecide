import DecisionBroker from "../../brokers/apiBroker.decisions";
import { useQueryClient, useMutation, useQuery } from "@tanstack/react-query";
import { Decision } from "../../models/decisions/decision";

export const decisionService = {
    useCreateDecision: () => {
        const broker = new DecisionBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (decision: Decision) => {
                const date = new Date();
                decision.createdDate = decision.updatedDate = date;

                return broker.PostDecisionAsync(decision);
            },
            onSuccess: (variables: Decision) => {
                queryClient.invalidateQueries({ queryKey: ["DecisionGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["DecisionGetById", { id: variables.id }] });
            }
        });
    },
    useModifyDecision: () => {
        const broker = new DecisionBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (decision: Decision) => {
                const date = new Date();
                decision.updatedDate = date;

                return broker.PutDecisionAsync(decision);
            },
            onSuccess: (data: Decision) => {
                queryClient.invalidateQueries({ queryKey: ["DecisionGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["DecisionGetById", { id: data.id }] });
            }
        });
    },
};