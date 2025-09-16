import { Container, Row, Col } from "react-bootstrap";
import { PatientSearch } from "../components/patientSearch/patientSearch";

export const PatientSearchPage = () => {

    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <PatientSearch />
                </Col>
            </Row>
        </Container>
    );
}