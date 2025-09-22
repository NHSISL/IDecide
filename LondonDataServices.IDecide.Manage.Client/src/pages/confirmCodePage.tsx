import ConfirmCode from "../components/confirmCode/confirmCode";
import { Card, Container } from "nhsuk-react-components";
import { Patient } from "../models/patients/patient";
import { PowerOfAttourney } from "../models/powerOfAttourneys/powerOfAttourney";
import { useLocation } from "react-router-dom";

export const ConfirmCodePage = () => {
    const location = useLocation();
    const createdPatient = location.state?.createdPatient as Patient;
    const powerOfAttorney = location.state?.powerOfAttorney as PowerOfAttourney | undefined;

    return (
        <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Confirm Their Code</Card.Heading>
                    <Card.Description>
                        <ConfirmCode
                            createdPatient={createdPatient}
                            powerOfAttorney={powerOfAttorney}
                        />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};

export default ConfirmCodePage;