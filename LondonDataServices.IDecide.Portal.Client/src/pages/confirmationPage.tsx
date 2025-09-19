import { Container } from "react-bootstrap";
import Confirmation from "../components/confirmation/confirmation";
import {useStep} from "../hooks/useStep";

export const ConfirmationPage = () => {
    const { selectedOption, nhsNumber, createdPatient, powerOfAttorney } = useStep();


    return (
        <Container style={{ padding: 20 }}>
            <Confirmation
                selectedOption={selectedOption}
                nhsNumber={nhsNumber}
                createdPatient={createdPatient}
                powerOfAttorney={powerOfAttorney}
            />
        </Container>
    );
};

export default ConfirmationPage;