import { Container } from "react-bootstrap";
import PositiveConfirmation from "../components/positiveConfirmation/positiveConfirmation";
import { PatientCodeRequest } from "../models/patients/patientCodeRequest";

export const SendCodePage = () => {
    const goToConfirmCode = (createdPatient: PatientCodeRequest) => {
        console.log("Code generated for patient:", createdPatient);
    };

    return (
        <Container style={{ padding: 20 }}>
            <PositiveConfirmation goToConfirmCode={goToConfirmCode} />
        </Container>
    );
};