import { useMsal } from "@azure/msal-react";
import ImpersonationContextBroker from "../../brokers/apiBroker.ImpersonationContext";
import { useQueryClient, useMutation, useQuery, useInfiniteQuery } from "@tanstack/react-query";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";


export const impersonationContextService = {
    useCreateimpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (impersonationContext: ImpersonationContext) => {
                const date = new Date();
                impersonationContext.createdDate = impersonationContext.updatedDate = date;
                impersonationContext.createdBy = impersonationContext.updatedBy = msal.accounts[0].username;

                return broker.PostImpersonationContextAsync(impersonationContext);
            },

            onSuccess: (variables: ImpersonationContext) => {
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllImpersonationContext: (query: string) => {
        const broker = new ImpersonationContextBroker();

        return useQuery({
            queryKey: ["ImpersonationContextGetAll", { query: query }],
            queryFn: () => broker.GetAllImpersonationContextAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllImpersonationContextPages: (query: string) => {
        const broker = new ImpersonationContextBroker();

        return useInfiniteQuery({
            queryKey: ["ImpersonationContextGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetImpersonationContextFirstPagesAsync(query)
                }
                return broker.GetImpersonationContextSubsequentPagesAsync(pageParam)
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage ?? null,
        });
    },

    useModifyImpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (impersonationContext: ImpersonationContext) => {
                const date = new Date();
                impersonationContext.updatedDate = date;
                impersonationContext.updatedBy = msal.accounts[0].username;

                return broker.PutImpersonationContextAsync(impersonationContext);
            },

            onSuccess: (data: ImpersonationContext) => {
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetById", { id: data.id }] });
            }
        });
    },

    useRemoveImpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: string) => {
                return broker.DeleteImpersonationContextByIdAsync(id);
            },

            onSuccess: (data: { id: string }) => {
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetById", { id: data.id }] });
            }
        });
    },
}