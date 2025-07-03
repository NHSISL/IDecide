import { Card, Col, Container, Row } from "react-bootstrap";
import { Link } from "react-router-dom";
import BreadCrumbBase from "../../components/bases/layouts/BreadCrumb/BreadCrumbBase";

export const ConfigurationHome = () => {
    return (
        <Container fluid className="mt-4"> 
            <Row>
                <BreadCrumbBase
                    link="/"
                    backLink="Home"
                    currentLink="Configuration">
                </BreadCrumbBase>

                <Col className="mt-3">
                    <Card>
                        <Card.Header as="h5">
                            <Link to={'/configuration/lookups'}>Lookups</Link>
                        </Card.Header>
                        <Card.Body>
                            <Card.Title>View, add, edit and remove lookups.</Card.Title>
                            <Card.Text>
                                This is the configuration page where you can manage lookup
                                values to drive the values for various dropdowns in the system.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col></Col>
                <Col></Col>
            </Row>
        </Container>
    )
}