import { useEffect, useState } from "react";
import { csvIdentificationRequestService } from "../../foundations/csvIdentificationRequestService";
import { CsvIdentificationRequestView } from "../../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";

type CsvIdentificationRequestViewServiceResponse = {
    mappedCsvIdentificationRequests: CsvIdentificationRequestView[] | undefined;
    pages: Array<{ data: CsvIdentificationRequestView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: CsvIdentificationRequestView[] }> } | undefined;
    refetch: () => void;
};

export const csvIdentificationRequestViewService = {
    useGetAllCsvIdentificationRequestsByEntraId: (searchTerm?: string, entraId?: string): CsvIdentificationRequestViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;
        query = query + `&$filter=(RequesterUserId eq '${entraId}' or RecipientUserId eq '${entraId}')`;

        if (searchTerm) {
            query = query + `&$filter=contains(RequesterFirstName,'${searchTerm}')`;
            
        }

        const response = csvIdentificationRequestService.useRetrieveAllCsvIdentificationRequestPages(query);
        const [mappedCsvIdentificationRequests, setMappedCsvIdentificationRequests] = useState<Array<CsvIdentificationRequestView>>();
        const [pages] = useState<Array<{ data: CsvIdentificationRequestView[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const validPages = (response.data.pages as Array<{ data: CsvIdentificationRequestView[] }>).filter(page => page.data).flatMap(x => x.data as CsvIdentificationRequestView[]);
                setMappedCsvIdentificationRequests(validPages);
            }
        }, [response.data]);

        return {
            mappedCsvIdentificationRequests,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    }
};