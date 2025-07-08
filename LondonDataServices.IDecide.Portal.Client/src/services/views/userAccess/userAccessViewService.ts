import { userAccessService } from "../../foundations/userAccessService";

export const userAccessViewService = {
    useGetAccessForUser: (entraId?: string) => {
        const query = `?$filter=UserId eq '${entraId}'`;
        return userAccessService.useGetAllUserAccess(query);
    },
    useGetOrgCodeAccessForUser: (entraId?: string) => {
        const query = `?$filter=UserId eq '${entraId}'&$select=orgCode`;
        return userAccessService.useGetAllUserAccess(query);
    }
}