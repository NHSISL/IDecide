import React from "react";
import { Container } from "react-bootstrap";

export const SearchByDetailsPage = () => {
    return (
        <Container style={{ padding: 20 }}>
        <h1>Search By Details Page</h1>
            <input type="text" placeholder="Enter NHS Number" />
        </Container>
    );
};
