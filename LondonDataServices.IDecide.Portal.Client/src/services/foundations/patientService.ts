import PatientBroker from "../../brokers/apiBroker.patients";
import PatientSearchBroker from "../../brokers/apiBroker.patientSearch";
import PatientCodeBroker from "../../brokers/apiBroker.patientCode";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Patient } from "../../models/patients/patient";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { PatientLookup } from "../../models/patients/patientLookup";

export const patientService = {
    useCreatePatient: async (
        patientLookup: PatientLookup,
        headers?: Record<string, string>
    ) => {
        const broker = new PatientSearchBroker();
        return await broker.PostPatientNhsNumberAsync(patientLookup, headers);
    },

    usePostPatientWithNotificationPreference: async (
        patient: PatientCodeRequest,
        headers?: Record<string, string>
    ) => {
        const broker = new PatientCodeBroker();
        return await broker.PostPatientWithNotificationPreference(patient, headers);
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

    useRetrieveAllPatients: (query: string) => {
        const broker = new PatientBroker();

        return useQuery({
            queryKey: ["PatientGetAll", { query: query }],
            queryFn: () => broker.GetAllPatientsAsync(query),
            staleTime: Infinity
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