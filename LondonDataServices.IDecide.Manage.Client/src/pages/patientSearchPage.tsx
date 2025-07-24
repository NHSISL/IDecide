import { Container, Row, Col } from "react-bootstrap";
import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const PatientSearchPage = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <SearchByDetails/>
                </Col>
            </Row>
        </Container>
    );
}