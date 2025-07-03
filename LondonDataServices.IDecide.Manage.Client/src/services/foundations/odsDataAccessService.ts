import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import OdsDataBroker from "../../brokers/apiBroker.odsData";

export const odsDataService = {
    useRetrieveAllOdsData: (query: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsDataApiGetAll", { query: query }],
            queryFn: () => broker.GetAllOdsDataAsync(query),
            staleTime: Infinity
        });
    },  

    useGetOdsChildren: (id: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsChildren", {query: id}],
            queryFn: () => broker.GetOdsChildrenByIdAsync(id),
            staleTime: Infinity
        })
    },

    useRetrieveAllOdsDataPages: (query: string) => {
        const broker = new OdsDataBroker();

        return useInfiniteQuery({
            queryKey: ["OdsDataGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetOdsDataFirstPagesAsync(query)
                }
                return broker.GetOdsDataSubsequentPagesAsync(pageParam)
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },
}
