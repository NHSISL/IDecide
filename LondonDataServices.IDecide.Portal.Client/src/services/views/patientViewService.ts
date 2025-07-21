import { useEffect, useState } from "react";
import { PatientView } from "../../models/views/patientView";
import { patientService } from "../foundations/patientService";

export const patientViewService = {
    useCreatePatient: () => {
        return patientService.useCreatePatient();
    },

    useGetPatientById: (id: string) => {
        const response = patientService.useRetrievePatientById(id);
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