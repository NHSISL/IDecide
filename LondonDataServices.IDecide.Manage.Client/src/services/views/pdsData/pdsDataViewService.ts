import { useEffect, useState } from "react";
import { PdsDataView } from "../../../models/views/components/pdsData/pdsDataView";
import { pdsDataService } from "../../foundations/pdsDataAccessService";

type PdsDataViewServiceResponse = {
    mappedPdsData: PdsDataView[] | undefined;
    pages: Array<{ data: PdsDataView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: PdsDataView[] }> } | undefined;
    refetch: () => void;
};

export const pdsDataViewService = {
    useCreatePdsData: () => {
        return pdsDataService.useCreatePdsData();
    },

    useGetAllPdsData: (getTestPatients: boolean, searchTerm?: string): PdsDataViewServiceResponse => {
        let query = ``;

        if (getTestPatients && searchTerm) {
            query = `?$filter=OrgCode eq 'ZZZ' and pseudoNhsNumber eq '${searchTerm}'`;
        } else if (getTestPatients) {
            query = `?$filter=OrgCode eq 'ZZZ'`;
        } else if (searchTerm) {
            query = `?$filter=pseudoNhsNumber eq '${searchTerm}'`;
        }

        const response = pdsDataService.useRetrieveAllPdsDataPages(query);
        const [mappedPdsData, setMappedPdsData] = useState<Array<PdsDataView>>();
        const [pages, setPages] = useState<Array<{ data: PdsDataView[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => page.data as PdsDataView[]);
                setMappedPdsData(allData);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedPdsData,
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