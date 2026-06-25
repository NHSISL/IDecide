import { Button, Container, Row, Col, Card } from "react-bootstrap";

export const UnauthorisedPage = () => {
    return (
        <Container
            className="d-flex justify-content-center align-items-center"
            style={{ height: '100vh' }}>
            <Row>
                <Col>
                    <Card className="text-center" style={{ maxWidth: '400px', margin: 'auto' }}>
                        <Card.Body>
                            <Card.Title className="mb-4">
                                <img
                                    src="/OneLondon_Logo_OneLondon_Logo_Blue.png"
                                    alt="London Data Service logo"
                                    height="70"
                                    width="216" />
                                <br />
                                <span style={{ marginLeft: "10px" }}>
                                    London Data Service <br />
                                    <strong className="hero-text"> Local Data Opt-Out</strong>
                                </span>
                            </Card.Title>
                            <Card.Text className="mb-4 align-items-left">
                                <p><strong>Access Denied</strong></p>
                                <p>
                                    Your account is not authorised to access this service.
                                    Please contact your administrator if you believe this is an error.
                                </p>
                            </Card.Text>
                            <Button href="/api/auth/login" variant="primary" className="me-3">
                                Try again
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default UnauthorisedPage;
