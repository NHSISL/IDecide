import React from "react";
import { Container, Alert, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export const ThankyouPage = () => {
    const navigate = useNavigate();

    const handleReturnHome = () => {
        navigate("/");
    };

    return (
        <Container>
            <Alert variant="success" style={{ fontSize: "1.25rem" }}>
                <h2 style={{ marginBottom: "1rem" }}>Thank You!</h2>
                <p>
                    Your preferences have been saved and your request is complete.
                </p>
                <p>
                    We appreciate your time. You will be notified when your preferences have been enacted.
                </p>
                <p>
                    You may now safely close this window or browser tab.
                </p>
                <div style={{ display: "flex", marginTop: 24 }}>
                    <Button variant="primary" onClick={handleReturnHome}>
                        Return Home
                    </Button>
                </div>
            </Alert>
        </Container>
    );
};

export default ThankyouPage;