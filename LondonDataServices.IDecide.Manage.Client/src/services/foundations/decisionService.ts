import { useInfiniteQuery } from '@tanstack/react-query';
import DecisionBroker from "../../brokers/apiBroker.decisions";

export const decisionService = {
    //useCreateDecision: () => {
    //    const broker = new DecisionBroker();

    //    return useMutation({
    //        mutationFn: (decision: Decision) => {
    //            return broker.PostDecisionAsync(decision);
    //        }
    //    });
    //},
    //useModifyDecision: () => {
    //    const broker = new DecisionBroker();
    //    const queryClient = useQueryClient();

    //    return useMutation({
    //        mutationFn: (decision: Decision) => {
    //            const date = new Date();
    //            decision.updatedDate = date;

    //            return broker.PostDecisionAsync(decision);
    //        },
    //        onSuccess: () => {
    //            queryClient.invalidateQueries({ queryKey: ["DecisionGetAll"] });
    //        }
    //    });
    //},
    useRetrieveAllDecisionPages: (query: string) => {
        const decisionBroker = new DecisionBroker();

        return useInfiniteQuery({
            queryKey: ["DecisionGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return decisionBroker.GetDecisionFirstPagesAsync(query)
                }
                return decisionBroker.GetDecisionSubsequentPagesAsync(pageParam)
            },
            staleTime: Infinity,
            initialPageParam: "",
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },
};