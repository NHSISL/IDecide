import { Container, Row, Col } from "react-bootstrap";
import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const PatientDetailsSearchPage = () => {
    const handleNextStep = () => {
    };

    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <SearchByDetails nextStep={handleNextStep} />
                </Col>
            </Row>
        </Container>
    );
}