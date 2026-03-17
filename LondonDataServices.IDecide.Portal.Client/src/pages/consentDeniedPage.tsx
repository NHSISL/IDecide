import { Col, Container, Row, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

export default function ConsentDeniedPage(): JSX.Element {
    const navigate = useNavigate();
    const { t: translate } = useTranslation();

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
                    <h1 style={{ fontSize: '4vw' }}>
                        {translate("consentDenied.consentNotGiven", "Consent Not Given")}
                    </h1>
                    <p>
                        {translate("consentDenied.consentNotGivenP1", "You have chosen not to give consent for NHS Login. Unfortunately, we cannot proceed without your consent.")}
                    </p>
                    <p>
                        {translate("consentDenied.consentNotGivenP2", "If you wish to use this service, please try logging in again and providing the required consent.")}
                    </p>
                    <Button
                        variant="outline-dark"
                        onClick={() => navigate('/home')}
                    >
                        {translate("consentDenied.consentNotGivenReturn", "RETURN TO HOME")}
                    </Button>
                </Col>
            </Row>
        </Container>
    );
}