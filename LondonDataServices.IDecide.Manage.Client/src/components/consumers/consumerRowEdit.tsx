import { FunctionComponent, useState } from "react";
import { Form, Row, Col, Button } from "react-bootstrap";
import { Consumer } from "../../models/consumers/consumer";

type ConsumerRowEditProps = {
    consumer: Consumer;
    onCancel?: () => void;
    onSave?: (updated: Consumer) => void;
};

const ConsumerRowEdit: FunctionComponent<ConsumerRowEditProps> = ({ consumer, onCancel, onSave }) => {
    const [name, setName] = useState(consumer.name || "");
    const [contactEmail, setContactEmail] = useState(consumer.contactEmail || "");
    const [contactNumber, setContactNumber] = useState(consumer.contactNumber || "");
    const [contactPerson, setContactPerson] = useState(consumer.contactPerson || "");
    const [entraId, setEntraId] = useState(consumer.entraId || "");

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const updatedConsumer: Consumer = {
            ...consumer,
            name,
            contactEmail,
            contactNumber,
            contactPerson,
            entraId
        };
        if (onSave) onSave(updatedConsumer);
    };

    return (
        <Form onSubmit={handleSubmit}>
            <h4>Edit Consumer</h4>
            <Row className="bg-light p-3 rounded">
                <Col md={6}>
                    <Form.Group className="mb-3" controlId="formName">
                        <Form.Label>Name</Form.Label>
                        <Form.Control
                            type="text"
                            value={name}
                            onChange={e => setName(e.target.value)}
                            required
                        />
                    </Form.Group>
                    <Form.Group className="mb-3" controlId="formContactEmail">
                        <Form.Label>Contact Email</Form.Label>
                        <Form.Control
                            type="email"
                            value={contactEmail}
                            onChange={e => setContactEmail(e.target.value)}
                            required
                        />
                    </Form.Group>
                    <Form.Group className="mb-3" controlId="formContactNumber">
                        <Form.Label>Contact Number</Form.Label>
                        <Form.Control
                            type="text"
                            value={contactNumber}
                            onChange={e => setContactNumber(e.target.value)}
                            required
                        />
                    </Form.Group>
                </Col>
                <Col md={6}>
                    <Form.Group className="mb-3" controlId="formContactPerson">
                        <Form.Label>Contact Name</Form.Label>
                        <Form.Control
                            type="text"
                            value={contactPerson}
                            onChange={e => setContactPerson(e.target.value)}
                            required
                        />
                    </Form.Group>
                    <Form.Group className="mb-3" controlId="formEntraId">
                        <Form.Label>Entra Id</Form.Label>
                        <Form.Control
                            type="text"
                            value={entraId}
                            onChange={e => setEntraId(e.target.value)}
                            required
                        />
                    </Form.Group>
                </Col>
            </Row>
            <div className="mt-3">
                <Button type="submit" variant="primary">
                    Save
                </Button>
                <Button
                    type="button"
                    variant="secondary"
                    onClick={onCancel}
                    className="ms-2"
                >
                    Cancel
                </Button>
            </div>
            <br />
        </Form>
    );
};

export default ConsumerRowEdit;