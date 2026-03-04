import { Container, Alert, Col, Row } from "react-bootstrap";
import { useTranslation } from "react-i18next";

export const ThankyouPage = () => {
    const { t: translate } = useTranslation();

    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>

                    <h1 style={{ marginBottom: "1rem", fontWeight: "bold", fontSize: "2.5rem" }}>
                        {translate("ThankyouScreen.thankyou")}
                    </h1>

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
                            privacy notice
                        </a>
                        &nbsp;for more information on how and when your choice will be applied.
                    </p>
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                </Col>
            </Row>
        </Container>
    );
};

export default ThankyouPage;