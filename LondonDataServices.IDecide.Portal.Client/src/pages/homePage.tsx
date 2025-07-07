import React from "react";
import { Container, Row, Col } from "react-bootstrap";
import { Button, Card, Checkboxes, Radios } from "nhsuk-react-components";
import { useNavigate } from "react-router-dom";

export const HomePage = () => {
    const navigate = useNavigate();

    return (
        <Container className="mt-4">
            <Row>
                HomePage
                <Button onClick={() => navigate("/optOut")}>Start</Button>
            </Row>
        </Container>
    );
};