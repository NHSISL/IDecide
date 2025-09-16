import PositiveConfirmation from "../components/positiveConfirmation/positiveConfirmation";
import { Card, Container } from "nhsuk-react-components";
import { useLocation } from "react-router-dom";
import { Patient } from "../models/patients/patient";

export const SendCodePage = () => {
    const location = useLocation();
    const createdPatient = location.state?.createdPatient as Patient;

    return (
         <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Positive Confimation Notification</Card.Heading>
                    <Card.Description>
                        <PositiveConfirmation createdPatient={createdPatient} />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};
