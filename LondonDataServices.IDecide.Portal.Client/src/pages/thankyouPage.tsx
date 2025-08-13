import { Container, Alert, Button, Col, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

export const ThankyouPage = () => {
    const navigate = useNavigate();
    const { t: translate } = useTranslation();

    const handleReturnHome = () => {
        navigate("/");
    };

    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
                    <Alert variant="success" style={{ fontSize: "1.25rem" }}>
                        <h2 style={{ marginBottom: "1rem" }}>{translate("ThankyouScreen.title")}</h2>
                        <p>
                            {translate("ThankyouScreen.preferencesSaved")}
                        </p>
                        <p>
                            {translate("ThankyouScreen.appreciation")}
                        </p>
                        <p>
                            {translate("ThankyouScreen.safeToClose")}
                        </p>
                        <div style={{ display: "flex", marginTop: 24 }}>
                            <Button variant="primary" onClick={handleReturnHome}>
                                {translate("ThankyouScreen.returnHome")}
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