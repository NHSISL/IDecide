import { useEffect, useState } from "react";
import { userAccessService } from "../../foundations/userAccessService";
import { UserAccessView } from "../../../models/views/components/userAccess/userAccessView";

type UserAccessViewServiceResponse = {
    mappedUserAccess: UserAccessView[] | undefined;
    pages: Array<{ data: UserAccessView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: UserAccessView[] }> } | undefined;
    refetch: () => void;
};

export const userAccessViewService = {
    useCreateUserAccess: () => {
        return userAccessService.useCreateUserAccess();
    },

    useGetDistinctUsers: () => {
        const response = userAccessService.useRetrieveDistinctUsers();
        return { ...response }
    },

    useGetDistinctUsersOdata: (searchTerm?: string): UserAccessViewServiceResponse => {
        let query = ``;

        if (searchTerm) {
            query = query + `?$filter=contains(surname,'${searchTerm}') or contains(givenName,'${searchTerm}') or contains(email,'${searchTerm}')`;
        }

        const response = userAccessService.useRetrieveAllUserAccessPages(query);
        const [mappedUserAccess, setMappedUserAccess] = useState<UserAccessView[]>([]);
        const [pages, setPages] = useState<Array<{ data: UserAccessView[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => page.data as UserAccessView[]);

                const groupedData = allData.reduce((acc, user) => {
                    const key = `${user.UserId}-${user.displayName}-${user.email}`;
                    if (!acc[key]) {
                        acc[key] = user;
                    }
                    return acc;
                }, {} as Record<string, UserAccessView>);

                setMappedUserAccess(Object.values(groupedData));
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedUserAccess,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetAccessForUser: (entraId?: string) => {
        const query = `?$filter=UserId eq '${entraId}'`;
        return userAccessService.useRetrieveAllUserAccess(query);
    },
};