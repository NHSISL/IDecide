import ConfirmDetails from "../components/confirmDetails/confirmDetails";
import { Card, Container } from "nhsuk-react-components";

export const ConfirmDetailsPage = () => {
    return (
        <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Confirm Patient Details</Card.Heading>
                    <Card.Description>
                        <ConfirmDetails />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};

export default ConfirmDetailsPage;