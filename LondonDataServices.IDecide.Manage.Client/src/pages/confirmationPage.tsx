import { Container } from "react-bootstrap";
import Confirmation from "../components/confirmation/confirmation";
import { useLocation } from "react-router-dom";
import { Patient } from "../models/patients/patient";

export const ConfirmationPage = () => {
    const location = useLocation();
    const { selectedOption, createdPatient, powerOfAttorney } = location.state || {};

    return (
        <Container fluid style={{ padding: 20 }}>
            <Confirmation
                selectedOption={selectedOption}
                createdPatient={createdPatient as Patient}
                powerOfAttorney={powerOfAttorney}
            />
        </Container>
    );
};

export default ConfirmationPage;