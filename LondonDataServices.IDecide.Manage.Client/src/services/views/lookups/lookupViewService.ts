import { useEffect, useState } from "react";
import { lookupService } from "../../foundations/lookupService";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { Lookup } from "../../../models/lookups/lookup";

type LookupViewServiceResponse = {
    mappedLookups: LookupView[] | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: LookupView[] }> } | undefined;
    refetch: () => void;
};

export const lookupViewService = {
    useCreateLookup: () => {
        return lookupService.useCreatelookup();
    },

    useGetAllLookups: (searchTerm?: string): LookupViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(name,'${searchTerm}')`;
        }

        const response = lookupService.useRetrieveAllLookupPages(query);
        const [mappedLookups, setMappedLookups] = useState<Array<LookupView>>();

        useEffect(() => {
            if (response.data) {
                const lookups = response.data.pages[0].data.map((lookup: Lookup) =>
                    new LookupView(
                        lookup.id,
                        lookup.name,
                        lookup.value,
                        lookup.groupName,
                        lookup.createdBy,
                        lookup.createdDate,
                        lookup.updatedBy,
                        lookup.updatedDate,
                    ));

                setMappedLookups(lookups);
            }
        }, [response.data]);

        return {
            mappedLookups, ...response
        }
    },

    useGetLookupById: (id: string) => {
        const query = `?$filter=id eq ${id}`;
        const response = lookupService.useRetrieveAllLookupPages(query);
        const [mappedLookup, setMappedLookup] = useState<LookupView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const lookup = response.data.pages[0].data[0];
                const lookupView = new LookupView(
                    lookup.id,
                    lookup.name,
                    lookup.value,
                    lookup.groupName,
                    lookup.createdBy,
                    lookup.createdDate,
                    lookup.updatedBy,
                    lookup.updatedDate
                );

                setMappedLookup(lookupView);
            }
        }, [response.data]);

        return {
            mappedLookup,
            ...response
        };
    },

    useUpdateLookup: () => {
        return lookupService.useModifyLookup();
    },

    useRemoveLookup: () => {
        return lookupService.useRemoveLookup();
    },
};