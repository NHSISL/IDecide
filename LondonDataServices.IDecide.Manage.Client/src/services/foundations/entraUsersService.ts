import { useQuery } from '@tanstack/react-query';
import UsersBroker from '../../brokers/apiBroker.entra.users';

export const UsersService = {
    useSearchUsers: (searchTerm : string) => {
        const UsersBroker = new UsersBroker();
        return useQuery({
            queryKey: ["UsersSearch", { query: searchTerm }],
            queryFn: async () => await UsersBroker.FilterUsersAsync(searchTerm),
            staleTime: Infinity
        });
    }
};