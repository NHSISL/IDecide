import { Container } from "react-bootstrap";
import {useStep} from "../hooks/useStep";
import ConfirmationNhsLogin from "../components/confirmation/confirmationNhsLogin";

export const ConfirmationNhsLoginPage = () => {
    const { selectedOption, nhsNumber, createdPatient } = useStep();


    return (
        <Container style={{ padding: 20 }}>
            <ConfirmationNhsLogin
                selectedOption={selectedOption}
                nhsNumber={nhsNumber}
                createdPatient={createdPatient}
            />
        </Container>
    );
};

export default ConfirmationNhsLoginPage;