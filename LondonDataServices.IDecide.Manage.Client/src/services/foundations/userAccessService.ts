import { useMsal } from "@azure/msal-react";
import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import UserAccessBroker from "../../brokers/apiBroker.userAccess";
import { UserAccess } from "../../models/userAccess/userAccess";

export const userAccessService = {

    useCreateUserAccess: () => {
        const broker = new UserAccessBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (userAccess: UserAccess) => {
                const date = new Date();
                userAccess.createdDate = userAccess.updatedDate = date;
                userAccess.createdBy = userAccess.updatedBy = msal.accounts[0].username;

                return broker.PostUserAccessBulkAsync(userAccess);
            },
            onSuccess: (variables: UserAccess) => {
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetAll"]});
                queryClient.invalidateQueries({ queryKey: ["DistinctUsers"]});
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveDistinctUsers: () => { 
        const broker = new UserAccessBroker();

        return useQuery({
            queryKey: ["DistinctUsers"],
            queryFn: () => broker.GetDistinctUsers(),
            staleTime: Infinity
        })
    },

    useRetrieveAllUserAccess: (query: string) => {
        const broker = new UserAccessBroker();

        return useQuery({
            queryKey: ["UserAccessGetAll", { query: query }],
            queryFn: () => broker.GetAllUserAccessAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllUserAccessPages: (query: string) => {
        const userAccessBroker = new UserAccessBroker();
        return useInfiniteQuery({
            queryKey: ["UserAccessGetAll", { query: query }],
            queryFn: ({ pageParam  }) => {
                if (!pageParam) {
                    return userAccessBroker.GetUserAccessFirstPagesAsync(query);
                }
                return userAccessBroker.GetUserAccessSubsequentPagesAsync(pageParam);
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage) => lastPage.nextPage ?? null,
        });
    },

    useModifyUserAccess: () => {
        const broker = new UserAccessBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (userAccess: UserAccess) => {
                const date = new Date();
                userAccess.updatedDate = date;
                userAccess.updatedBy = msal.accounts[0].username;

                return broker.PutUserAccessAsync(userAccess);
            },

            onSuccess: (data: UserAccess) => {
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetAll"]});
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetById", { id: data.id }] });
            }
        });
    },

    useRemoveUserAccess: () => {
        const broker = new UserAccessBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: string) => {
                return broker.DeleteUserAccessByIdAsync(id);
            },

            onSuccess: (data: { id: string }) => {
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["DistinctUsers"]});
                queryClient.invalidateQueries({ queryKey: ["UserAccessGetById", { id: data.id }] });
            }
        });
    },
}