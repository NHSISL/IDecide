import { Card, Col, Container, ListGroup, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import HeaderComponent from "../../components/layouts/header";

const WebsitePrivacyNoticePage = () => {
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
                <Row className="justify-content-center">
                    <Col md={10} lg={8}>
                        <Card>
                            <Card.Body>
                                <Card.Title as="h1">Website Privacy Notice</Card.Title>
                                <Card.Text>
                                    This privacy notice explains how we collect, use and protect the personal information you
                                    provide when you use this website. It is written for patients and members of the public and
                                    follows clear, plain language so you can quickly understand what we do with your data.
                                </Card.Text>

                                <h2 className="h5">Who we are</h2>
                                <p>
                                    The ONE London Opt In / Opt Out portal is provided on behalf of NHS organisations across London.
                                    The portal enables patients to exercise their right to choose how their confidential information
                                    is used for purposes beyond their individual care. The service is managed by the London Data Service (LDS)
                                    in partnership with NHS London Integrated Care Boards.
                                </p>

                                <h2 className="h5">What information we collect</h2>
                                <ListGroup>
                                    <ListGroup.Item>
                                        <strong>Personal details:</strong> Information such as your NHS number, name, date of birth,
                                        and postcode are collected to verify your identity and apply your preference correctly.
                                    </ListGroup.Item>
                                </ListGroup>

                                <h2 className="h5 mt-4">How we use your information</h2>
                                <p>
                                    The information you provide is used solely to:
                                </p>
                                <ul>
                                    <li>verify your identity to process your opt-in or opt-out request,</li>
                                    <li>record and maintain your current data sharing preference,</li>
                                    <li>support auditing and compliance with NHS information governance standards,</li>
                                    <li>ensure the website is secure and functioning properly.</li>
                                </ul>

                                <h2 className="h5">Updates to this notice</h2>
                                <p>
                                    This notice may be updated from time to time to reflect changes in legislation or the way the
                                    service operates. The latest version will always be available on this page.
                                </p>


                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </>
    );
};

export default WebsitePrivacyNoticePage;