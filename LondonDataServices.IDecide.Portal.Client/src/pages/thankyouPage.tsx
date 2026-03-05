import { Button } from "nhsuk-react-components";
import { Container, Alert, Col, Row } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

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
                    <Alert variant="light">
                        <strong style={{ marginBottom: "1rem", fontWeight: "bold", fontSize: "1.5rem" }}>
                            {translate("ThankyouScreen.preferencesSaved")}
                        </strong>
                        <br /><br />
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

                    <br />

                    <h2 style={{ marginBottom: "1rem", fontWeight: "bold", fontSize: "1.5rem" }}>More Information</h2>
                    <p>
                        Visit our&nbsp;
                        <a
                            href="/websitePrivacyNotice"
                            target="_blank"
                            rel="noopener noreferrer"
                            style={{ textDecoration: "underline" }}
                        >
                            {translate("ThankyouScreen.privacy")} 
                        </a>
                        &nbsp;
                        {translate("ThankyouScreen.moreinfo")}
                    </p>
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                </Col>
            </Row>
        </Container>
    );
};

export default ThankyouPage;