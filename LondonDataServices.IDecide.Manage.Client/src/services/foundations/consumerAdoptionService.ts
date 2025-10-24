import { useInfiniteQuery} from '@tanstack/react-query';
import ConsumerAdoptionBroker from '../../brokers/apiBroker.consumerAdoptions';

export const consumerAdoptionService = {
    useRetrieveAllConsumerAdoptionPages: (query: string) => {
        const consumerAdoptionBroker = new ConsumerAdoptionBroker();

        return useInfiniteQuery({
            queryKey: ["ConsumerGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return consumerAdoptionBroker.GetConsumerAdoptionFirstPagesAsync(query)
                }
                return consumerAdoptionBroker.GetConsumerAdoptionSubsequentPagesAsync(pageParam)
            },
            staleTime: Infinity,
            initialPageParam: "",
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },
};