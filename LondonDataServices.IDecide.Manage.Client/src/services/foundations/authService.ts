import { useQuery, useMutation } from '@tanstack/react-query';
import AuthSessionBroker from '../../brokers/apiBroker.authSession';
import { SessionData } from '../../models/sessions/SessionData';

export const authService = {
    useGetSession: () => {
        const broker = new AuthSessionBroker();

        return useQuery<SessionData>({
            queryKey: ['authSession'],
            queryFn: async () => await broker.GetSessionDetailsAsync(),
            staleTime: 30 * 60 * 1000,
            refetchInterval: 60 * 1000,
            retry: false,
        });

        
    },
    useLogout: () => {
        const broker = new AuthSessionBroker();

        return useMutation({
            mutationFn: async () => await broker.PostLogoutAsync(),
        });
    }
};