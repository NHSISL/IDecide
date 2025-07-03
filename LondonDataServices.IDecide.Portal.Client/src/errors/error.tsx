import { Col, Container, Row, Button, Accordion } from "react-bootstrap";
import { isRouteErrorResponse, useNavigate, useRouteError } from "react-router-dom";

export default function ErrorPage(): JSX.Element {
    const navigate = useNavigate();
    const error = useRouteError();
    let errorMessage: string;

    if (isRouteErrorResponse(error)) {
        // error is type `ErrorResponse`
        errorMessage = error.data?.message || error.statusText;
    } else if (error instanceof Error) {
        errorMessage = error.message;
    } else if (typeof error === 'string') {
        errorMessage = error;
    } else {
        console.error('Unknown error:', error);
        errorMessage = 'An unknown error has occurred';
    }

    return (
        <Container fluid className="d-flex vh-100">
            <div style={{ position: 'absolute', top: '10px', right: '10px' }}>
                <img src="/OneLondon_Logo_OneLondon_Logo_Blue.png" alt="London Data Service logo" height="65" width="208" />
            </div>
            <Row className="m-auto">
                <Col md={12} lg={12} className="p-4">
                    <h1 style={{ fontSize: '4vw' }}>Sorry, something went wrong</h1>
                    <p>We apologise for the inconvenience, contact support if the problem persists.</p>
                    <Accordion>
                        <Accordion.Item eventKey="0">
                            <Accordion.Header>TECHNICAL DETAILS</Accordion.Header>
                            <Accordion.Body>
                                <i>{errorMessage}</i>
                            </Accordion.Body>
                        </Accordion.Item>
                    </Accordion>
                    <br />
                    <Button variant="outline-dark" onClick={() => navigate('/')}>GO BACK TO SITE</Button>
                </Col>
            </Row>
        </Container>
    );
}