import { useMsal } from "@azure/msal-react";
import { Button, Container, Row, Col, Card } from "react-bootstrap";
import { MsalConfig } from "../../authConfig";
import { useLocation } from "react-router-dom";

export const LoginUnAuthorisedComponent = () => {

    const { instance } = useMsal();
    const location = useLocation();

    const handleLoginRedirect = async () => {
        const loginRequest = MsalConfig.loginRequest;
        loginRequest.redirectUri = location.pathname;
        instance.loginRedirect(loginRequest).catch((error) => console.log(error));
    };

    return (
        <Container className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
            <Row>
                <Col>
                    <Card className="text-center" style={{ maxWidth: '400px', margin: 'auto' }}>
                        <Card.Body>
                            <Card.Title className="mb-4 ">
                                <img src="/OneLondon_Logo_OneLondon_Logo_Blue.png" alt="London Data Service logo" height="70" width="216" />
                                <br />
                                <span style={{ marginLeft: "10px" }}>
                                    One London Data Service <br />
                                    <strong className="hero-text"> Re-Identification</strong>
                                </span>

                            </Card.Title>
                            <Card.Text className="mb-4 align-items-left" >
                                <p>Welcome to the One London Re-Identification Portal.</p>
                                <p>Please sign in to continue.</p>
                            </Card.Text>
                            <Button onClick={handleLoginRedirect} className="me-3">Sign in</Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}

export default LoginUnAuthorisedComponent;