import { useState, useEffect } from "react";
import { Consumer } from "../../models/consumers/consumer";
import { consumerService } from "../foundations/consumerService";

type ConsumerViewServiceResponse = {
    pages: Array<{ data: Consumer[] }> | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: Consumer[] }> } | undefined;
    refetch: () => void;
};

export const consumerViewService = {
    useCreatePatientConsumer: () => {
        const mutation = consumerService.useCreatePatientConsumer();

        return {
            mutate: async (consumer: Consumer, options?: { onSuccess?: () => void, onError?: (error: unknown) => void }) => {
                try {
                    await mutation.mutateAsync(consumer);
                    options?.onSuccess?.();
                } catch (error) {
                    options?.onError?.(error);
                }
            }
        };
    },

    useGetAllConsumers: (searchTerm?: string): ConsumerViewServiceResponse => {
        let query = `?$orderby=createdDate asc`;

        if (searchTerm) {
            query += `&$filter=contains(Name,'${searchTerm}')`;
        }

        const response = consumerService.useRetrieveAllConsumerPages(query);
        const [pages, setPages] = useState<Array<{ data: Consumer[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useUpdateConsumer: () => {
        return consumerService.useModifyConsumer();
    },

    useRemoveConsumer: () => {
        return consumerService.useRemoveConsumer();
    },
};