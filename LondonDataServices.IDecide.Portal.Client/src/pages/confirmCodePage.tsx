import React from "react";
import { Container } from "react-bootstrap";

export const ConfirmCodePage = () => {
    return (
        <Container style={{ padding: 20 }}>
        <h1>Confirm With Code Page</h1>
            <input type="text" placeholder="Enter Code" />
        </Container>
    );
};
