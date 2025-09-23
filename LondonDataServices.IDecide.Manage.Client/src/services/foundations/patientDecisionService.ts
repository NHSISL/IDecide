import PatientDecisionBroker from "../../brokers/apiBroker.patientDecisions";
import { useQueryClient, useMutation } from "@tanstack/react-query";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";

export const patientDecisionService = {
    useCreatePatientDecision: async(
        decision: PatientDecision,
    ) => {
        const broker = new PatientDecisionBroker();

        return await broker.PostPatientDecisionAsync(decision);
    },

    useModifyPatientDecision: () => {
        const broker = new PatientDecisionBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (decision: PatientDecision) => {
                const date = new Date();
                decision.updatedDate = date;

                return broker.PostPatientDecisionAsync(decision);
            },
            onSuccess: () => {
                queryClient.invalidateQueries({ queryKey: ["DecisionGetAll"] });
            }
        });
    },
};