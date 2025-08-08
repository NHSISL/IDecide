import { Container } from "react-bootstrap";
import { useStep } from "../hooks/useStep";
import { ConfirmCode } from "../components/confirmCode/confirmCode";

export const ConfirmCodePage = () => {
    const { createdPatient } = useStep();

    return (
        <Container>
            <ConfirmCode createdPatient={createdPatient} />
        </Container>
    );
};

export default ConfirmCodePage;