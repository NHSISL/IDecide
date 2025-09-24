import { useInfiniteQuery} from '@tanstack/react-query';
import PatientBroker from "../../brokers/apiBroker.patients";
import PatientSearchBroker from "../../brokers/apiBroker.patientSearch";
import PatientCodeBroker from "../../brokers/apiBroker.patientCode";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Patient } from "../../models/patients/patient";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { PatientLookup } from "../../models/patients/patientLookup";

export const patientService = {
    useCreatePatient: () => {
        const broker = new PatientSearchBroker();

        return useMutation({
            mutationFn: (patientLookup: PatientLookup) => {
                return broker.PostPatientNhsNumberAsync(patientLookup);
            },
        });
    },

    usePostPatientWithNotificationPreference: () => {
        const broker = new PatientCodeBroker();

        return useMutation({
            mutationFn: (patientLookup: PatientCodeRequest) => {
                return broker.PostPatientWithNotificationPreference(patientLookup);
            },
        });
    },

    useConfirmCode: () => {
        const broker = new PatientCodeBroker();
        return useMutation({
            mutationFn: (request: PatientCodeRequest) => {
                return broker.ConfirmPatientCodeAsync(request.nhsNumber!, request.verificationCode!);
            }
        });
    },

    useCreatePatientByDetails: () => {
        const broker = new PatientBroker();

        return useMutation({
            mutationFn: (patientLookup: PatientLookup) => {
                return broker.PostPatientDetailsAsync(patientLookup);
            }
        });
    },

    useRetrievePatientById: (nhsNumber: string) => {
        const broker = new PatientBroker();

        return useQuery<Patient>({
            queryKey: ["PatientGetById", { id: nhsNumber }],
            queryFn: () => broker.GetPatientByIdAsync(nhsNumber),
            staleTime: Infinity
        });
    },

    useRetrieveAllPatientPages: (query: string) => {
        const patientBroker = new PatientBroker();

        return useInfiniteQuery({
            queryKey: ["PatientGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return patientBroker.GetPatientFirstPagesAsync(query)
                }
                return patientBroker.GetPatientSubsequentPagesAsync(pageParam)
            },
            staleTime: Infinity,
            initialPageParam: "",
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },
};