import { MutationCache, QueryCache, QueryClient } from '@tanstack/react-query';
import axios from 'axios';

export const queryClientGlobalOptions = new QueryClient({
    defaultOptions: {
        queries: {
            retry: false
        }
    },
    queryCache: new QueryCache({
        onError: (error: Error, query) => {
            if (axios.isAxiosError(error) && error.response?.status === 401) {
                const isSessionQuery = query.queryKey[0] === 'authSession';

                if (!isSessionQuery) {
                    window.location.href = '/api/auth/login';
                    return;
                }

                return;
            }

            console.log("An unknown error has occurred, please refresh the page and try again.");
            throw error;
        }
    }),
    mutationCache: new MutationCache({
        onSuccess: () => {
           // toastSuccess("Saved.");
            console.log("Saved.");
        },
        onError: (error: Error) => {
            if (error) {
                console.log("An unknown error has occurred, please try again.");
                //toastError("An unknown error has occurred, please try again.");
            } else {
                //toastWarning("Your record has not been saved, please correct and try again.");
                console.log("Your record has not been saved, please correct and try again.")
                // throw new ApiValidationError(error?.response?.data?.errors);
            }

            throw error;
        }
    })
});