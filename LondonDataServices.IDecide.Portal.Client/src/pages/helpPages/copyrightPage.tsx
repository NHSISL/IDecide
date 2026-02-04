import { Container, Card, Row, Col } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

const CopyrightPage = () => {
    const navigate = useNavigate();
    const { configuration } = useFrontendConfiguration();

    return (
        <>
            <Container className="py-4" style={{ maxWidth: 800 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    Back
                </button>
                <Card className="shadow-sm">
                    <Card.Body>
                        <Row>
                            <Col>
                                <h2 className="mb-3">Copyright</h2>
                                <address className="mb-4">
                                    <strong>NEL ICB</strong><br />
                                    &copy; 2025 NEL NHS<br />

                                </address>
                                <hr />
                                <h3 className="mb-3">Software Details</h3>
                                <dl className="row mb-0">
                                    <dt className="col-sm-4">Application Name:</dt>
                                    <dd className="col-sm-8">{configuration.application}</dd>
                                    <dt className="col-sm-4">Published Version Number:</dt>
                                    <dd className="col-sm-8">{configuration.version}</dd>
                                    <dt className="col-sm-4">Current Environment:</dt>
                                    <dd className="col-sm-8">{configuration.environment}</dd>
                                </dl>
                            </Col>
                        </Row>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default CopyrightPage;