import { Container } from "react-bootstrap";
import PositiveConfirmation from "../components/positiveConfirmation/positiveConfirmation";
import { GenerateCodeRequest } from "../models/patients/generateCodeRequest";

export const SendCodePage = () => {
    const goToConfirmCode = (createdPatient: GenerateCodeRequest) => {
        console.log("Code generated for patient:", createdPatient);
    };

    return (
        <Container style={{ padding: 20 }}>
            <PositiveConfirmation goToConfirmCode={goToConfirmCode} />
        </Container>
    );
};