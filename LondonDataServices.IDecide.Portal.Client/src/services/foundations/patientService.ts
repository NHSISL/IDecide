import PatientBroker from "../../brokers/apiBroker.patients";
import { useQueryClient, useMutation, useQuery } from "@tanstack/react-query";
import { Patient } from "../../models/patients/patient";

export const patientService = {

    useCreatePatient: () => {
        const broker = new PatientBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (patient: Patient) => {
                const date = new Date();
                patient.createdDate = patient.updatedDate = date;
               
                return broker.PostPatientAsync(patient);
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

    useModifyPatient: () => {
        const broker = new PatientBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (patient: Patient) => {
                const date = new Date();
                patient.updatedDate = date;

                return broker.PutPatientAsync(patient);
            },
            onSuccess: (data: Patient) => {
                queryClient.invalidateQueries({ queryKey: ["PatientGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["PatientGetById", { id: data.id }] });
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
};