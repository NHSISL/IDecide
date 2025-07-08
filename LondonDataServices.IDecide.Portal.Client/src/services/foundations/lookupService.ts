import { useQuery } from '@tanstack/react-query';
import LookupBroker from "../../brokers/apiBroker.lookups";

export const lookupService = {
    useRetrieveAllLookups: (query: string) => {
        const broker = new LookupBroker();

        return useQuery({
            queryKey: ["LookupGetAll", { query: query }],
            queryFn: () => broker.GetAllLookupsAsync(query),
            staleTime: Infinity
        });
    }
}