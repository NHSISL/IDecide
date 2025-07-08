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
            query = query + `&$filter=contains(Reason,'${searchTerm}') or ` +
                `contains(RequesterFirstName,'${searchTerm}') or ` +
                `contains(RequesterLastName,'${searchTerm}') or ` +
                `contains(RequesterEmail,'${searchTerm}') or ` +
                `contains(RecipientFirstName,'${searchTerm}') or ` +
                `contains(RecipientLastName,'${searchTerm}') or ` +
                `contains(RecipientEmail,'${searchTerm}')`;
        }

        const response = impersonationContextService.useRetrieveAllImpersonationContextPages(query);
        const [mappedImpersonationContexts, setMappedImpersonationContexts] = useState<Array<ImpersonationContextView>>();
        const [pages, setPages] = useState<Array<{ data: ImpersonationContextView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const impersonationContexts: Array<ImpersonationContextView> = [];
                response.data.pages.forEach((x: { data: ImpersonationContextView[] }) => {
                    x.data.forEach((impersonationContext: ImpersonationContextView) => {
                        impersonationContexts.push(new ImpersonationContextView(
                            impersonationContext.id,
                            impersonationContext.requesterFirstName,
                            impersonationContext.requesterLastName,
                            impersonationContext.requesterEmail,
                            impersonationContext.recipientFirstName,
                            impersonationContext.recipientLastName,
                            impersonationContext.recipientEmail,
                            impersonationContext.reason,
                            impersonationContext.purpose,
                            impersonationContext.organisation,
                            impersonationContext.isApproved,
                            impersonationContext.identifierColumn,
                            impersonationContext.createdBy,
                            impersonationContext.createdDate,
                            impersonationContext.updatedBy,
                            impersonationContext.updatedDate,
                        ));
                    });
                });

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
                const impersonationContext = response.data.pages[0].data[0];
                const impersonationContextView = new ImpersonationContextView(
                    impersonationContext.id,
                    impersonationContext.requesterFirstName,
                    impersonationContext.requesterLastName,
                    impersonationContext.requesterEmail,
                    impersonationContext.recipientFirstName,
                    impersonationContext.recipientLastName,
                    impersonationContext.recipientEmail,
                    impersonationContext.reason,
                    impersonationContext.purpose,
                    impersonationContext.organisation,
                    impersonationContext.isApproved,
                    impersonationContext.identifierColumn,
                    impersonationContext.createdBy,
                    impersonationContext.createdDate,
                    impersonationContext.updatedBy,
                    impersonationContext.updatedDate,
                );

                setMappedImpersonationContext(impersonationContextView);
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