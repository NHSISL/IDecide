import { Card, Container } from "nhsuk-react-components";
import OptInOptOut from "../components/optInOptOut/optInOptOut";
import { useLocation } from "react-router-dom";
import { Patient } from "../models/patients/patient";

export const OptInOutPage = () => {
    const location = useLocation();
    const createdPatient = location.state?.createdPatient as Patient;


    return (
        <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Decision</Card.Heading>
                    <Card.Description>
                       <OptInOptOut createdPatient={createdPatient} />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};