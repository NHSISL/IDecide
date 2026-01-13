import { Col, Container, Row, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function ConsentDeniedPage(): JSX.Element {
    const navigate = useNavigate();

    return (
        <Container fluid className="d-flex vh-100">
            <div style={{ position: 'absolute', top: '10px', right: '10px' }}>
                <img
                    src="/OneLondon_Logo_OneLondon_Logo_Blue.png"
                    alt="London Data Service logo"
                    height="65"
                    width="208"
                />
            </div>
            <Row className="m-auto">
                <Col md={12} lg={12} className="p-4">
                    <h1 style={{ fontSize: '4vw' }}>Consent Not Given</h1>
                    <p>
                        You have chosen not to give consent for NHS Login. Unfortunately, we cannot proceed without your consent.
                    </p>
                    <p>
                        If you wish to use this service, please try logging in again and provide the required consent.
                    </p>
                    <Button
                        variant="outline-dark"
                        onClick={() => navigate('/home')}
                    >
                        RETURN TO HOME
                    </Button>
                </Col>
            </Row>
        </Container>
    );
}