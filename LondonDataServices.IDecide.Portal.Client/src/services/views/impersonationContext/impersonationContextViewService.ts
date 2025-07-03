import { useEffect, useState } from "react";
import { impersonationContextService } from "../../foundations/impersonationContextService";
import { ImpersonationContextView } from "../../../models/views/components/impersonationContext/ImpersonationContextView";

type ImpersonationContextViewServiceResponse = {
    mappedImpersonationContexts: ImpersonationContextView[] | undefined;
    pages: Array<{ data: ImpersonationContextView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: ImpersonationContextView[] }> } | undefined;
    refetch: () => void;
};

export const impersonationContextViewService = {
    useCreateImpersonationContext: () => {
        return impersonationContextService.useCreateimpersonationContext();
    },

    useGetAllImpersonationContexts: (searchTerm?: string): ImpersonationContextViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(projectName,'${searchTerm}')`
        }

        const response = impersonationContextService.useRetrieveAllImpersonationContextPages(query);
        const [mappedImpersonationContexts, setMappedImpersonationContexts] = useState<Array<ImpersonationContextView>>();
        const [pages, setPages] = useState<Array<{ data: ImpersonationContextView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const impersonationContexts = response.data.pages.flatMap(x => x.data as ImpersonationContextView[]);
                setMappedImpersonationContexts(impersonationContexts);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedImpersonationContexts,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetImpersonationContextById: (id: string) => {
        const query = `?$filter=id eq ${id}`;
        const response = impersonationContextService.useRetrieveAllImpersonationContextPages(query);
        const [mappedImpersonationContext, setMappedImpersonationContext] = useState<ImpersonationContextView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const impersonationContext = response.data.pages[0].data[0] as ImpersonationContextView;
                setMappedImpersonationContext(impersonationContext);
            }
        }, [response.data]);

        return {
            mappedImpersonationContext,
            ...response
        };
    },

    useUpdateImpersonationContext: () => {
        return impersonationContextService.useModifyImpersonationContext();
    },

    useRemoveImpersonationContext: () => {
        return impersonationContextService.useRemoveImpersonationContext();
    },
};