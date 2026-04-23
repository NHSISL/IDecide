import { useEffect, useRef } from 'react';
import { authService } from '../services/foundations/authService';

const SESSION_CHECK_INTERVAL_MS = 10 * 60 * 1000;

export function useAuth() {
    const query = authService.useGetSession();
    const logoutMutation = authService.useLogout();
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const hasLoggedOutRef = useRef(false);
    const logoutMutationRef = useRef(logoutMutation);
    const lastLoggedMinuteRef = useRef<number>(-1);

    useEffect(() => {
        logoutMutationRef.current = logoutMutation;
    }, [logoutMutation]);

    useEffect(() => {
        if (intervalRef.current) {
            clearInterval(intervalRef.current);
        }

        if (query.data?.expiresAt) {
            const checkSession = () => {
                const expiryTime = new Date(query.data.expiresAt).getTime();
                const currentTime = new Date().getTime();
                const timeRemaining = expiryTime - currentTime;

                if (timeRemaining <= 0 && !hasLoggedOutRef.current) {
                    hasLoggedOutRef.current = true;
                    logoutMutationRef.current.mutateAsync().finally(() => {
                        window.location.href = '/';
                    });
                } else if (timeRemaining > 0) {
                    const hours = Math.floor(timeRemaining / 3600000);
                    const minutes = Math.floor((timeRemaining % 3600000) / 60000);
                    const totalMinutes = hours * 60 + minutes;

                    if (lastLoggedMinuteRef.current !== totalMinutes) {
                        lastLoggedMinuteRef.current = totalMinutes;
                        console.log(`Session expires in ${hours} hours and ${minutes} minutes`);
                    }
                }
            };

            checkSession();

            intervalRef.current = setInterval(checkSession, SESSION_CHECK_INTERVAL_MS);
        }

        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
            }
        };
    }, [query.data?.expiresAt]);

    return {
        isAuthenticated: query.isSuccess,
        isLoading: query.isLoading,
        sessionData: query.data,
        error: query.error,
    };
}