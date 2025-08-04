import React from "react";
import PositiveConfirmation from "../components/positiveConfirmation/positiveConfirmation";
import { Card, Container } from "nhsuk-react-components";

export const SendCodePage = () => {
    return (
         <Container fluid>
            <Card cardType="feature">
                <Card.Content>
                    <Card.Heading>Positive Confimation Notification</Card.Heading>
                    <Card.Description>
                       <PositiveConfirmation/>
                    </Card.Description>
                </Card.Content>
            </Card>
        </Container>
    );
};
