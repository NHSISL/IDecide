import { Container, Alert, Button, Col, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export const ThankyouPage = () => {
    const navigate = useNavigate();

    const handleReturnHome = () => {
        navigate("/");
    };

    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
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
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                </Col>
            </Row>
        </Container>
    );
};

export default ThankyouPage;