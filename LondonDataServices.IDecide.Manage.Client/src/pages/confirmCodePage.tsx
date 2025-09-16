import ConfirmCode from "../components/confirmCode/confirmCode";
import { Card, Container } from "nhsuk-react-components";
import { Patient } from "../models/patients/patient";
import { useLocation } from "react-router-dom";

export const ConfirmCodePage = () => {
    const location = useLocation();
    const createdPatient = location.state?.createdPatient as Patient;

    return (
         <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Confirm Their Code</Card.Heading>
                    <Card.Description>
                        <ConfirmCode createdPatient={createdPatient} />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};

export default ConfirmCodePage;