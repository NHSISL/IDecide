import { Container, Row, Col, Card } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import HeaderComponent from "../../components/layouts/header";

const AboutPage = () => {
    const navigate = useNavigate();

    return (
        <>
            <HeaderComponent />
            <Container style={{ padding: 20 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    Back
                </button>
                <div style={{ marginTop: 24 }}>
                    <h1>About OneLondon</h1>
                    <Card className="mb-4">
                        <Card.Body>
                            <h2 className="h5">What is OneLondon?</h2>
                            <p>
                                OneLondon is a collaborative of London’s five Integrated Care Systems (health and care partnerships formed by NHS organisations and local councils) and the London Ambulance Service. We are supported by NHS England (London region), the Greater London Authority and London’s three Health Innovation Networks.
                            </p>
                            <p>
                                For more information on OneLondon please visit our <a href="https://www.onelondon.online/" target="_blank" rel="noopener noreferrer">website</a>.
                            </p>
                        </Card.Body>
                    </Card>

                    <Card className="mb-4">
                        <Card.Body>
                            <h2 className="h5">London Secure Data Environment</h2>
                            <p>
                                Through NHS England funding from the Sub-National Secure Data Environment Network programme, and building on existing initiatives in the Capital, OneLondon are developing a world leading resource for health and care improvement known as the London SDE (Secure Data Environment).
                            </p>
                            <p>
                                Londoners are centre stage of this work to ensure the London SDE meets public expectations and builds trust and confidence. You can read more about our work with Londoners, including the OneLondon Citizens’ Advisory Group, <a href="https://www.onelondon.online/citizens-advisory-group/" target="_blank" rel="noopener noreferrer">here</a>.
                            </p>
                            <p>
                                The London SDE has two key elements:
                            </p>
                            <ul>
                                <li>The London Data Service</li>
                                <li>The London Analytics Platform</li>
                            </ul>
                        </Card.Body>
                    </Card>

                    <Row>
                        <Col md={6} className="mb-4">
                            <Card>
                                <Card.Body>
                                    <h3 className="h6">What is London Data Service?</h3>
                                    <p>
                                        The London Data Service securely collects patient information from a range of places such as GP surgeries and hospitals. It then organises and stores this information ready to be used by other approved systems such as the London Care Record or the London Analytics Platform.
                                    </p>
                                    <p>
                                        It is stored as identifiable data so it can be used to support the direct care you receive. It is also stored as de-identified data which means it does not include information which could tell someone who you are (such as names, addresses or dates of birth). Only de-identified data is used for service planning and research purposes. We only ever share the minimum necessary data for these purposes.
                                    </p>
                                </Card.Body>
                            </Card>
                        </Col>
                        <Col md={6} className="mb-4">
                            <Card>
                                <Card.Body>
                                    <h3 className="h6">What is the London Analytics Platform?</h3>
                                    <p>
                                        The London Analytics Platform uses data from the London Data Service to:
                                    </p>
                                    <ul>
                                        <li>Support health and care teams to provide direct/proactive care.</li>
                                        <li>Help health and care organisations to plan services.</li>
                                        <li>Help health and care academic teams or industry partners to perform research and development.</li>
                                    </ul>
                                </Card.Body>
                            </Card>
                        </Col>
                    </Row>
                </div>
            </Container>
        </>
    );
};

export default AboutPage;