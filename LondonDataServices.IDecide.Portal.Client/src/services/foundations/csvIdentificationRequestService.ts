import { useMsal } from "@azure/msal-react";
import CsvIdentificationRequestBroker from "../../brokers/apiBroker.csvIdentificationRequest";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";
import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const csvIdentificationRequestService = {
    useCreateCsvIdentificationRequest: () => {
        const broker = new CsvIdentificationRequestBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (csvIdentificationRequest: CsvIdentificationRequest) => {
                const date = new Date();
                csvIdentificationRequest.createdDate = csvIdentificationRequest.updatedDate = date;
                csvIdentificationRequest.createdBy = csvIdentificationRequest.updatedBy = msal.accounts[0].username;

                return broker.PostCsvIdentificationRequestAsync(csvIdentificationRequest);
            },

            onSuccess: (variables: CsvIdentificationRequest) => {
                queryClient.invalidateQueries({ queryKey: ["CsvIdentificationRequestGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["CsvIdentificationRequestById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllCsvIdentificationRequest: (query: string) => {
        const broker = new CsvIdentificationRequestBroker();

        return useQuery({
            queryKey: ["CsvIdentificationRequestGetAll", { query: query }],
            queryFn: () => broker.GetAllCsvIdentificationRequestAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllCsvIdentificationRequestPages: (query: string) => {
        const broker = new CsvIdentificationRequestBroker();

        return useInfiniteQuery({
            queryKey: ["CsvIdentificationRequestGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetCsvIdentificationRequestFirstPagesAsync(query)
                }
                return broker.GetCsvIdentificationRequestSubsequentPagesAsync(pageParam)
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },

    useModifyCsvIdentificationRequest: () => {
        const broker = new CsvIdentificationRequestBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (csvIdentificationRequest: CsvIdentificationRequest) => {
                const date = new Date();
                csvIdentificationRequest.updatedDate = date;
                csvIdentificationRequest.updatedBy = msal.accounts[0].username;

                return broker.PutCsvIdentificationRequestAsync(csvIdentificationRequest);
            },
            onSuccess: (data: CsvIdentificationRequest) => {
                queryClient.invalidateQueries({ queryKey: ["CsvIdentificationRequestGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["CsvIdentificationRequestGetById", { id: data.id }] });
            }
        });
    },

    useSelectCsvIdentificationByCsvIdentificationRequestIdRequest: (csvIdentificationRequestId: string) => {
        const broker = new CsvIdentificationRequestBroker();

        return useQuery<CsvIdentificationRequest>({
            queryKey: ["GetCsvIdentificationRequestById", { csvIdentificationRequestId: csvIdentificationRequestId }],
            queryFn: () => broker.GetCsvIdentificationRequestByIdAsync(csvIdentificationRequestId),
            staleTime: Infinity
        });
    },
}