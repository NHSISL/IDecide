import { useEffect, useState } from "react";
import { PatientView } from "../../models/views/patientView";
import { patientService } from "../foundations/patientService";

export const patientViewService = {
    useCreatePatient: () => {
        return patientService.useCreatePatient();
    },

    useGetPatientByNhsNumber: (nhsNumber: string) => {
        const response = patientService.useRetrievePatientById(nhsNumber);
        const [mappedPatient, setMappedPatient] = useState<PatientView>();

        useEffect(() => {
            if (response.data) {
                setMappedPatient(response.data as PatientView);
            }
        }, [response.data]);

        return {
            mappedPatient,
            ...response
        };
    },

    useUpdatePatient: () => {
        return patientService.useModifyPatient();
    },
};