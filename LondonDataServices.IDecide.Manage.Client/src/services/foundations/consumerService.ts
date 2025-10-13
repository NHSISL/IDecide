import { useInfiniteQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import ConsumerBroker from "../../brokers/apiBroker.consumers";
import { useMsal } from '@azure/msal-react';
import { Consumer } from '../../models/consumers/consumer';
import { ApiError } from '../../types/apiError';

export const consumerService = {
    useCreatePatientConsumer: () => {
        const broker = new ConsumerBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation<Consumer, ApiError, Consumer, unknown>({
            mutationFn: (consumer: Consumer) => {
                const date = new Date();
                consumer.createdDate = consumer.updatedDate = date;
                consumer.createdBy = consumer.updatedBy = msal.accounts[0].username;
                return broker.PostConsumerAsync(consumer);
            },
            onSuccess: (variables: Consumer) => {
                queryClient.invalidateQueries({ queryKey: ["ConsumerGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["ConsumerGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllConsumerPages: (query: string) => {
        const decisionBroker = new ConsumerBroker();

        return useInfiniteQuery({
            queryKey: ["ConsumerGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return decisionBroker.GetConsumerFirstPagesAsync(query)
                }
                return decisionBroker.GetConsumerSubsequentPagesAsync(pageParam)
            },
            staleTime: Infinity,
            initialPageParam: "",
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },

    useModifyConsumer: () => {
        const consumerBroker = new ConsumerBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (consumer: Consumer) => {
                const date = new Date();
                consumer.updatedDate = date;
                consumer.updatedBy = msal.accounts[0].username;

                return consumerBroker.PutConsumerAsync(consumer);
            },

            onSuccess: (data: { id: string }) => {
                queryClient.invalidateQueries({ queryKey: ["ConsumerGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["consumerGetById", { id: data.id }] });
            }
        });
    },

    useRemoveConsumer: () => {
        const broker = new ConsumerBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: string) => {
                return broker.DeleteConsumerByIdAsync(id);
            },
            onSuccess: (data: { id: string }) => {
                queryClient.invalidateQueries({ queryKey: ["ConsumerGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["ConsumerGetById", { id: data.id }] });
            }
        });
    },
};