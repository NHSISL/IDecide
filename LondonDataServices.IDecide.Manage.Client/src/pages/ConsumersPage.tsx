import { Container, Row, Col } from "react-bootstrap";
import ConsumerTable from "../components/consumers/consumerTable";

export const ConsumersPage = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <h3>Consumers</h3>
                    <ConsumerTable />
                    
                </Col>
            </Row>
        </Container>
    );
}