import { useEffect, useState } from "react";
import { lookupService } from "../../foundations/lookupService";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { UseQueryResult } from "@tanstack/react-query";
import { Lookup } from "../../../models/lookups/lookup";

type LookupViewServiceResponse = UseQueryResult<Lookup[], Error> & {
    mappedLookups: LookupView[]
}

export const lookupViewService = {
    useGetAllLookups: (searchTerm?: string, groupName?: string): LookupViewServiceResponse => {
        let query = `?$orderby=createdDate desc&$filter=groupName eq '${groupName}'`;

        if (searchTerm) {
            query = query + `&$filter=contains(value,'${searchTerm}')`;
        }

        const response = lookupService.useRetrieveAllLookups(query);
        const [mappedLookups, setMappedLookups] = useState<Array<LookupView>>([]);

        useEffect(() => {
            if (response.data) {
                const lookups = response.data as LookupView[];
                setMappedLookups(lookups);
            }
        }, [response.data]);

        return {
            mappedLookups, ...response
        }
    },
};