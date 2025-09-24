import { useLocation } from "react-router-dom";
import ConfirmDetails from "../components/confirmDetails/confirmDetails";
import { Card, Container } from "nhsuk-react-components";
import { Patient } from "../models/patients/patient";
import { PowerOfAttourney } from "../models/powerOfAttourneys/powerOfAttourney";

export const ConfirmDetailsPage = () => {
    const location = useLocation();
    const createdPatient = location.state?.createdPatient as Patient;
    const poaModel = location.state?.poaModel as PowerOfAttourney | undefined;

    return (
        <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Confirm Patient Details</Card.Heading>
                    <Card.Description>
                        <ConfirmDetails createdPatient={createdPatient} powerOfAttorney={poaModel} />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};

export default ConfirmDetailsPage;