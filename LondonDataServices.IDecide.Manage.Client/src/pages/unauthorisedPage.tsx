import { Button, Container, Row, Col, Card } from "react-bootstrap";

export const UnauthorisedPage = () => {
    return (
        <Container
            className="d-flex justify-content-center align-items-center"
            style={{ height: '100vh' }}>
            <Row>
                <Col>
                    <Card className="text-center" style={{ maxWidth: '600px', margin: 'auto' }}>
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
                                    <strong className="hero-text"> Local Data Opt-Out Management Portal</strong>
                                </span>
                                <br /><br />
                                <p>Healthcare Worker Login</p>
                            </Card.Title>
                            <Card.Text className="mb-4 align-items-left">
                                <p className="text-danger"><strong>Access Denied</strong></p>
                                <p>
                                   
                                        Your account is  <strong>not authorised</strong> to access this service. <br /> <br />
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
