import { Container } from "react-bootstrap";
import ConfirmCode from "../components/confirmCode/confirmCodepppp";
import { useStep } from "../components/context/stepContext";

export const ConfirmCodePage = () => {
    const { createdPatient } = useStep();

    return (
        <Container>
            <ConfirmCode createdPatient={createdPatient} />
        </Container>
    );
};

export default ConfirmCodePage;