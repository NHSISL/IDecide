import { useEffect, useState } from "react";
import { csvIdentificationRequestService } from "../../foundations/csvIdentificationRequestService";
import { CsvIdentificationRequestView } from "../../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";

type CsvIdentificationRequestViewServiceResponse = {
    mappedCsvIdentificationRequests: CsvIdentificationRequestView[] | undefined;
    pages: Array<{ data: CsvIdentificationRequestView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: CsvIdentificationRequestView[] }> } | undefined;
    refetch: () => void;
};

export const csvIdentificationRequestViewService = {
    useCreateCsvIdentificationRequest: () => {
        return csvIdentificationRequestService.useCreateCsvIdentificationRequest();
    },

    useGetAllCsvIdentificationRequests: (searchTerm?: string): CsvIdentificationRequestViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(RequesterFirstName,'${searchTerm}')`;
        }

        const response = csvIdentificationRequestService.useRetrieveAllCsvIdentificationRequestPages(query);
        const [mappedCsvIdentificationRequests, setMappedCsvIdentificationRequests] = useState<Array<CsvIdentificationRequestView>>();
        const [pages, setPages] = useState<Array<{ data: CsvIdentificationRequestView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const csvIdentificationRequests: Array<CsvIdentificationRequestView> = [];
                response.data.pages.forEach((x: { data: CsvIdentificationRequestView[] }) => {
                    x.data.forEach((csvIdentificationRequest: CsvIdentificationRequestView) => {
                        csvIdentificationRequests.push(new CsvIdentificationRequestView(
                            csvIdentificationRequest.id,
                            csvIdentificationRequest.requesterFirstName,
                            csvIdentificationRequest.requesterLastName,
                            csvIdentificationRequest.requesterEmail,
                            csvIdentificationRequest.recipientFirstName,
                            csvIdentificationRequest.recipientLastName,
                            csvIdentificationRequest.reason,
                            csvIdentificationRequest.purpose,
                            csvIdentificationRequest.organisation,
                            csvIdentificationRequest.data,
                            csvIdentificationRequest.sha256Hash,
                            csvIdentificationRequest.identifierColumn,
                            csvIdentificationRequest.filepath,
                            csvIdentificationRequest.createdBy,
                            csvIdentificationRequest.createdDate,
                            csvIdentificationRequest.updatedBy,
                            csvIdentificationRequest.updatedDate,
                        ));
                    });
                });

                setMappedCsvIdentificationRequests(csvIdentificationRequests);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedCsvIdentificationRequests,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetCsvIdentificationRequestById: (id: string) => {
        const query = `?$filter=id eq ${id}`;
        const response = csvIdentificationRequestService.useRetrieveAllCsvIdentificationRequestPages(query);
        const [mappedCsvIdentificationRequest, setMappedCsvIdentificationRequest] = useState<CsvIdentificationRequestView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const csvIdentificationRequest = response.data.pages[0].data[0];
                const csvIdentificationRequestView = new CsvIdentificationRequestView(
                    csvIdentificationRequest.id,
                    csvIdentificationRequest.requesterFirstName,
                    csvIdentificationRequest.requesterLastName,
                    csvIdentificationRequest.requesterEmail,
                    csvIdentificationRequest.recipientFirstName,
                    csvIdentificationRequest.recipientLastName,
                    csvIdentificationRequest.reason,
                    csvIdentificationRequest.purpose,
                    csvIdentificationRequest.organisation,
                    csvIdentificationRequest.data,
                    csvIdentificationRequest.sha256Hash,
                    csvIdentificationRequest.identifierColumn,
                    csvIdentificationRequest.filepath,
                    csvIdentificationRequest.createdBy,
                    csvIdentificationRequest.createdDate,
                    csvIdentificationRequest.updatedBy,
                    csvIdentificationRequest.updatedDate,
                );

                setMappedCsvIdentificationRequest(csvIdentificationRequestView);
            }
        }, [response.data]);

        return {
            mappedCsvIdentificationRequest,
            ...response
        };
    },

    useUpdateCsvIdentificationRequest: () => {
        return csvIdentificationRequestService.useModifyCsvIdentificationRequest();
    },

    useRemoveCsvIdentificationRequest: () => {
        return csvIdentificationRequestService.useRemoveCsvIdentificationRequest();
    },
};