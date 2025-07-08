import { useEffect, useState } from "react";
import { AccessAudit } from "../../../models/accessAudit/accessAudit";
import { accessAuditService } from "../../foundations/accessAuditService";

type AccessAuditViewServiceResponse = {
    pages: Array<{ data: AccessAudit[] }> | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: AccessAudit[] }> } | undefined;
    refetch: () => void;
};

export const accessAuditViewService = {
    useGetAllAccessAuditByRequestId: (searchTerm?: string, requestId?: string): AccessAuditViewServiceResponse => {
        let query = `?$filter=requestId eq ${requestId}`;

        if (searchTerm) {
            query = query + ` and (contains(Email,'${searchTerm}'))`;
        }

        query = query + `&$orderby=createdDate desc`;

        const response = accessAuditService.useRetrieveAllAccessAusitPages(query);
        const [pages, setPages] = useState<Array<{ data: AccessAudit[] }>>([]);

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