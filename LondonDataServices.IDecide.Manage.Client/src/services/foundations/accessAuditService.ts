import { useInfiniteQuery, useQuery } from '@tanstack/react-query';
import AccessAuditBroker from '../../brokers/apiBroker.accessAudit';

export const accessAuditService = {

    useRetrieveAllAccessAudits: (query: string) => {
        const broker = new AccessAuditBroker();

        return useQuery({
            queryKey: ["AccessAuditGetAll", { query: query }],
            queryFn: () => broker.GetAllAccessAuditAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllAccessAusitPages: (query: string) => {
        const accessAuditBroker = new AccessAuditBroker();
        return useInfiniteQuery({
            queryKey: ["AccessAuditGetAll", { query: query }],
            queryFn: ({ pageParam  }) => {
                if (!pageParam) {
                    return accessAuditBroker.GetAccessAuditFirstPagesAsync(query);
                }
                return accessAuditBroker.GetAccessAuditSubsequentPagesAsync(pageParam);
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage) => lastPage.nextPage ?? null,
        });
    } 
}