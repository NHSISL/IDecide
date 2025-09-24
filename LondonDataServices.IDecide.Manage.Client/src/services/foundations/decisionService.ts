import { useInfiniteQuery } from '@tanstack/react-query';
import DecisionBroker from "../../brokers/apiBroker.decisions";

export const decisionService = {
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