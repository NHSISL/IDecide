import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";;
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";

const ConsumerTable = () => {
    return (
        <>
            <br /> <br />
            <Container className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" />Download History</Card.Header>
                    <Card.Body>

                        <Table striped bordered hover variant="light" responsive>
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Pow</th>
                                    <th>Pow</th>
                                    <th>Pow</th>
                                </tr>
                            </thead>
                            <tbody>

                            </tbody>
                        </Table>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default ConsumerTable;