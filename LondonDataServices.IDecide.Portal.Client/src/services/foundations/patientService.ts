import PatientBroker from "../../brokers/apiBroker.patients";
import { useQueryClient, useMutation, useQuery } from "@tanstack/react-query";
import { Patient } from "../../models/patients/patient";
import { GenerateCodeRequest } from "../../models/patients/generateCodeRequest";
import { ConfirmCodeRequest } from "../../models/patients/confirmCodeRequest";

export const patientService = {

    useCreatePatientByNhsNumber: () => {
        const broker = new PatientBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (patient: Patient) => {
                const date = new Date();
                patient.createdDate = patient.updatedDate = date;
               
                return broker.PostPatientNhsNumberAsync(patient);
            },
            onSuccess: (variables: Patient) => {
                queryClient.invalidateQueries({ queryKey: ["PatientGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PatientGetById", { id: variables.id }] });
            }
        });
    },

    useCreatePatientByDetails: () => {
        const broker = new PatientBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (patient: Patient) => {
                const date = new Date();
                patient.createdDate = patient.updatedDate = date;

                return broker.PostPatientDetailsAsync(patient);
            },
            onSuccess: (variables: Patient) => {
                queryClient.invalidateQueries({ queryKey: ["PatientGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PatientGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllPatients: (query: string) => {
        const broker = new PatientBroker();

        return useQuery({
            queryKey: ["PatientGetAll", { query: query }],
            queryFn: () => broker.GetAllPatientsAsync(query),
            staleTime: Infinity
        });
    },

    useGenerateCodeRequest: () => {
        const broker = new PatientBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (request: GenerateCodeRequest) => {
                return broker.PutGenerateCodeRequestAsync(request);
            },
            onSuccess: () => {
                queryClient.invalidateQueries({ queryKey: ["PatientGetAll"] });
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

    useConfirmCode: () => {
        const broker = new PatientBroker();
        return useMutation({
            mutationFn: (request: ConfirmCodeRequest) => {
                return broker.ConfirmPatientCodeAsync(request.nhsNumber, request.code);
            }
        });
    },
};