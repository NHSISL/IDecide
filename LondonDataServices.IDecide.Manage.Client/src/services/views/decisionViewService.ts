import { useState, useEffect } from "react";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { DecisionView } from "../../models/views/decisionView";
import { patientDecisionService } from "../foundations/patientDecisionService";
import { decisionService } from "../foundations/decisionService";
import { Decision } from "../../models/decisions/decision";

type DecisionViewServiceResponse = {
    mappedDecisions: DecisionView[] | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: DecisionView[] }> } | undefined;
    refetch: () => void;
};

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

    useUpdatePatientDecision: () => {
        return patientDecisionService.useModifyPatientDecision();
    },

    useGetAllDecisions: (patientId: string): DecisionViewServiceResponse => {
        let query = `?$orderby=createdDate desc&$expand=decisionType`;


        if (patientId && patientId.trim() !== "") {
            query += `&$filter=patientId eq ${patientId}`;
        }

        const response = decisionService.useRetrieveAllDecisionPages(query);
        const [mappedDecisions, setMappedDecisions] = useState<Array<DecisionView>>();

        useEffect(() => {
            if (response.data) {
                const decisions = response.data.pages[0].data.map((decision: Decision) =>
                        new DecisionView({
                            id: decision.id,
                            patientId: decision.patientId,
                            decisionTypeId: decision.decisionTypeId,
                            decisionChoice: decision.decisionChoice,
                            createdBy: decision.createdBy,
                            createdDate: decision.createdDate,
                            updatedBy: decision.updatedBy,
                            updatedDate: decision.updatedDate,
                            responsiblePersonGivenName: decision.responsiblePersonGivenName,
                            responsiblePersonSurname: decision.responsiblePersonSurname,
                            responsiblePersonRelationship: decision.responsiblePersonRelationship,
                            decisionType: decision.decisionType,
                            patient: decision.patient,
                            consumerAdoptions: decision.consumerAdoptions
                        })
                    );

                setMappedDecisions(decisions);
            }
        }, [response.data]);

        return {
            mappedDecisions,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },
};