import React from "react";
import { Container } from "react-bootstrap";

export const SearchByNhsNumberPage = () => {
    return (
        <Container style={{ padding: 20 }}>
            <input type="text" placeholder="Enter NHS Number" />
        </Container>
    );
};
