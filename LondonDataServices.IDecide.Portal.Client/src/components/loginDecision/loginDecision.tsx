import { useNavigate } from "react-router-dom";
import { Button, Card, Row, Col, Container } from "react-bootstrap";
import { useTranslation } from "react-i18next";

export const LoginDecision = () => {
    const navigate = useNavigate();
    const { t: translate } = useTranslation();

    return (
        <Container
            fluid
            className="d-flex justify-content-center align-items-center"
            style={{ minHeight: "80vh" }}
        >
            <Row
                className="w-100 justify-content-center flex-column align-items-center"
                xs={1}
                md={1}
                style={{ maxWidth: 1000 }}
            >
                <Col className="mb-4 d-flex justify-content-center">
                    <Card
                        className="flex-fill login-decision-card"
                        style={{
                            maxWidth: 1000,
                            width: "100%",
                            marginRight: 8,
                            marginLeft: 8,
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "space-between"
                        }}
                    >
                        <Card.Body className="d-flex flex-column justify-content-between align-items-start">
                            <div style={{ width: "100%" }}>
                                <Card.Title>
                                    <strong style={{ textAlign: "left", display: "block", fontSize: "1.5rem" }}>
                                        {translate("homepage.startButton")}
                                    </strong>
                                </Card.Title>
                                <Card.Text>
                                    <div style={{ textAlign: "left" }}>
                                        <p>
                                            {translate(
                                                "homepage.startButtonDescription",
                                                "Log in to securely record your own opt in or opt out preference for the use of your health data in research " +
                                                "through the London Data Service."
                                            )}
                                        </p>
                                        <p>
                                            {translate(
                                                "homepage.yourChoice",
                                                "Your choice will help determine how your information is used to support healthcare research and improvements across London."
                                            )}
                                        </p>
                                    </div>
                                </Card.Text>
                            </div>
                            <Button
                                className="nhsuk-button-blue mt-auto"
                                data-testid="start-login-button"
                                onClick={() => (window.location.href = "/nhs-optOut")}
                                style={{
                                    alignSelf: "flex-start",
                                    marginTop: 20
                                }}
                            >
                                {translate("homepage.continueToNextPage", "Continue")}
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>
                <Col className="mb-4 d-flex justify-content-center">
                    <Card
                        className="flex-fill login-decision-card"
                        style={{
                            maxWidth: 1000,
                            width: "100%",
                            marginRight: 8,
                            marginLeft: 8,
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "space-between",
                            minHeight: 320
                        }}
                    >
                        <Card.Body className="d-flex flex-column justify-content-between align-items-start">
                            <div style={{ width: "100%" }}>
                                <Card.Title>
                                    <strong style={{ textAlign: "left", display: "block", fontSize: "1.5rem" }}>
                                        {translate("homepage.startButtonOther")}
                                    </strong>
                                </Card.Title>
                                <Card.Text>
                                    <div style={{ textAlign: "left" }}>
                                        {translate(
                                            "homepage.startButtonOtherDescription",
                                            "Act on behalf of someone else to manage their NHS Data sharing preferences in the London Data Service, if you have the legal authority to represent them."
                                        )}
                                    </div>
                                </Card.Text>
                                <section
                                    style={{
                                        marginTop: "1rem",
                                        background: "#f0f4f5",
                                        padding: "0.7rem 1rem",
                                        borderRadius: "6px",
                                        fontSize: "0.97rem",
                                        textAlign: "left"
                                    }}
                                >
                                    <strong>
                                        {translate("homepage.beforeYouStartTitle")}
                                    </strong>
                                    <ul style={{ marginTop: "0.3rem", paddingLeft: "1.1rem" }}>
                                        <li>{translate("homepage.beforeYouStartList1")}</li>
                                        <li>{translate("homepage.beforeYouStartList2")}</li>
                                        <li>{translate("homepage.beforeYouStartList3")}</li>
                                    </ul>
                                </section>
                            </div>
                            <Button
                                data-testid="start-another-person-button"
                                onClick={() => { navigate("/optOut", { state: { powerOfAttorney: true } }); }}
                                style={{
                                    alignSelf: "flex-start",
                                    marginTop: 20
                                }}
                            >
                                {translate("homepage.continueToNextPage", "Continue")}
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default LoginDecision;