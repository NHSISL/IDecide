import { patientService } from "../foundations/patientService";
import { PatientView } from "../../models/views/patientView";
import { useState, useEffect } from "react";
import { Patient } from "../../models/patients/patient";

type PatientViewServiceResponse = {
    mappedPatients: PatientView[] | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: PatientView[] }> } | undefined;
    refetch: () => void;
};

type PatientSearchParams = {
    nhsNumber?: string;
    surname?: string;
    postCode?: string;
    dateOfBirth?: string;
};


export const patientViewService = {
    usePostPatientSearch: () => {
        return patientService.useCreatePatient();
    },
    usePostPatientDetails: () => {
        return patientService.useCreatePatient();
    },
    useAddPatientAndGenerateCode: () => {
        return patientService.usePostPatientWithNotificationPreference();
    },
    useConfirmCode: () => {
        return patientService.useConfirmCode();
    },

    useGetAllPatients: (searchParams?: PatientSearchParams): PatientViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchParams) {
            const { nhsNumber, surname, postCode, dateOfBirth } = searchParams;

            if (nhsNumber && nhsNumber.trim() !== "") {
                query += `&$filter=contains(nhsNumber,'${nhsNumber}')`;
            } else {
                const filters: string[] = [];
                if (surname && surname.trim() !== "") {
                    filters.push(`contains(surname,'${surname}')`);
                }
                if (postCode && postCode.trim() !== "") {
                    filters.push(`contains(postCode,'${postCode}')`);
                }
                if (dateOfBirth && dateOfBirth.trim() !== "") {
                    filters.push(`dateOfBirth eq ${dateOfBirth}`);
                }
                if (filters.length > 0) {
                    query += `&$filter=${filters.join(' and ')}`;
                }
            }
        }

        const response = patientService.useRetrieveAllPatientPages(query);
        const [mappedPatients, setMappedPatients] = useState<Array<PatientView>>();

        useEffect(() => {
            if (response.data) {
                const patients = response.data.pages[0].data.map((patient: Patient) =>
                    new PatientView(
                        patient.id!,
                        patient.nhsNumber,
                        patient.title,
                        patient.givenName,
                        patient.surname,
                        patient.dateOfBirth,
                        patient.gender,
                        patient.email,
                        patient.phone,
                        patient.address,
                        patient.postCode,
                        patient.validationCode,
                        patient.validationCodeExpiresOn,
                        patient.validationCodeMatchedOn,
                        patient.retryCount,
                        patient.notificationPreference,
                        patient.createdBy,
                        patient.createdDate,
                        patient.updatedBy,
                        patient.updatedDate
                    ));

                setMappedPatients(patients);
            }
        }, [response.data]);

        return {
            mappedPatients,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    }
};