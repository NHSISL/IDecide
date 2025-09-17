import { Card, Container } from "nhsuk-react-components";
import { PatientSearch } from "../components/patientSearch/patientSearch";

export const PatientSearchPage = () => {
    return (
        <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Patient Search</Card.Heading>
                    <Card.Description>
                    </Card.Description>
                    <PatientSearch />
                </Card.Content>
            </Card>
        </Container>
    );
};