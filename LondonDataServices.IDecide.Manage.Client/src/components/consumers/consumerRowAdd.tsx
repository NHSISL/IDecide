import { FunctionComponent, useState } from "react";
import { Form, Row, Col, Button } from "react-bootstrap";
import { Consumer } from "../../models/consumers/consumer";

type ConsumerRowAddProps = {
    onCancel?: () => void;
    onSave?: (consumer: Consumer) => void;
};

const ConsumerRowAdd: FunctionComponent<ConsumerRowAddProps> = ({ onCancel, onSave }) => {
    const [name, setName] = useState("");
    const [contactEmail, setContactEmail] = useState("");
    const [contactNumber, setContactNumber] = useState("");
    const [contactName, setContactName] = useState("");
    const [entraId, setEntraId] = useState("");

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const consumer: Consumer = {
            id: crypto.randomUUID(),
            name,
            contactEmail,
            contactNumber,
            contactPerson: contactName,
            entraId
        };
        if (onSave) onSave(consumer);
    };

    return (
        <Form onSubmit={handleSubmit}>
            <h4>Add Consumer</h4>
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
                    <Form.Group className="mb-3" controlId="formContactName">
                        <Form.Label>Contact Name</Form.Label>
                        <Form.Control
                            type="text"
                            value={contactName}
                            onChange={e => setContactName(e.target.value)}
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
                    Add
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

export default ConsumerRowAdd;