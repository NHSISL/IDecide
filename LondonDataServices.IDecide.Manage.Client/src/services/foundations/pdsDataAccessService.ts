import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import PdsDataBroker from "../../brokers/apiBroker.pdsData";
import { PdsData } from "../../models/pds/pdsData";

export const pdsDataService = {
    useCreatePdsData: () => {
        const broker = new PdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (pdsData: PdsData) => {
                return broker.PostPdsDataAsync(pdsData);
            },
            onSuccess: (variables: PdsData) => {
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllPdsData: (query: string) => {
        const broker = new PdsDataBroker();

        return useQuery({
            queryKey: ["PdsDataGetAll", { query: query }],
            queryFn: () => broker.GetAllPdsDataAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllPdsDataPages: (query: string) => {
        const broker = new PdsDataBroker();

        return useInfiniteQuery({
            queryKey: ["PdsDataGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetPdsDataFirstPagesAsync(query)
                }
                return broker.GetPdsDataSubsequentPagesAsync(pageParam)
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },

    useModifyPdsData: () => {
        const broker = new PdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (pdsData: PdsData) => {
                return broker.PutPdsDataAsync(pdsData);
            },
            onSuccess: (data: PdsData) => {
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetById", { id: data.id }] });
            }
        });
    },

    useRemovePdsData: () => {
        const broker = new PdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: string) => {
                return broker.DeletePdsDataByIdAsync(id);
            },
            onSuccess: (data: PdsData) => {
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PdsDataGetById", { id: data.id }] });
            }
        });
    },
}