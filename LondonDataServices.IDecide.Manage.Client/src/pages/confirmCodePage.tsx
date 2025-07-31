import ConfirmCode from "../components/confirmCode/confirmCode";
import { Card, Container } from "nhsuk-react-components";

export const ConfirmCodePage = () => {
    return (
         <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Confirm Their Code</Card.Heading>
                    <Card.Description>
                        <ConfirmCode />
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};

export default ConfirmCodePage;