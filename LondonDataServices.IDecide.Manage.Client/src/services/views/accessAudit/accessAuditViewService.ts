import { useEffect, useState } from "react";
import { AccessAudit } from "../../../models/accessAudit/accessAudit";
import { accessAuditService } from "../../foundations/accessAuditService";

type AccessAuditViewServiceResponse = {
    mappedAccessAudit: AccessAudit[] | undefined;
    pages: Array<{ data: AccessAudit[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    totalPages: number;
    data: { pages: Array<{ data: AccessAudit[] }> } | undefined;
    refetch: () => void;
};

export const accessAuditViewService = {
    useGetAllAccessAuditData: (searchTerm?: string): AccessAuditViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(pseudoIdentifier,'${searchTerm}') or 
                 contains(Email,'${searchTerm}') or 
                 contains(message,'${searchTerm}') or 
                 contains(organisation,'${searchTerm}') or 
                 contains(Reason,'${searchTerm}')`;
        }

        const response = accessAuditService.useRetrieveAllAccessAusitPages(query);
        const [mappedAccessAudit, setMappedAccessAudit] = useState<Array<AccessAudit>>();
        const [pages, setPages] = useState<Array<{ data: AccessAudit[] }>>([]);
        const [totalPages, setTotalPages] = useState<number>(0);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => page.data as AccessAudit[]);
                setMappedAccessAudit(allData);
                setPages(response.data.pages);

                // Paging
                const itemsPerPage = response.data.pages[0]?.data.length || 1;
                const totalItems = allData.length;
                setTotalPages(Math.ceil(totalItems / itemsPerPage));
            }
        }, [response.data]);

        return {
            mappedAccessAudit,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            totalPages,
            data: response.data,
            refetch: response.refetch
        };
    },
};