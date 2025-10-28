import { useState, useEffect } from "react";
import { ConsumerAdoption } from "../../models/consumerAdoptions/consumerAdoption";
import { consumerAdoptionService } from "../foundations/consumerAdoptionService";

type ConsumerAdoptionViewServiceResponse = {
    pages: Array<{ data: ConsumerAdoption[] }> | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: ConsumerAdoption[] }> } | undefined;
    refetch: () => void;
};

export const consumerAdoptionViewService = {
    useGetAllConsumerAdoptions: (decisionId?: string): ConsumerAdoptionViewServiceResponse => {
       
        const query = decisionId
            ? `?$orderby=createdDate asc&$expand=Consumer,Decision&$filter=decisionId eq ${decisionId}`
            : ""; // No query if decisionId is not provided

        const response = consumerAdoptionService.useRetrieveAllConsumerAdoptionPages(query);
        const [pages, setPages] = useState<Array<{ data: ConsumerAdoption[] }>>([]);

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
};