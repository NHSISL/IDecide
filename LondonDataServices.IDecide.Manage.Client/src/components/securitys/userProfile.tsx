import { ReactElement, useState } from "react"
import { Button, Card, ListGroup, Modal, NavDropdown } from "react-bootstrap";
import moment from "moment";
import { useAuth } from "../../hooks/useAuth";

export const UserProfile = (): ReactElement => {
    const { sessionData } = useAuth();
    const [showModal, setShowModal] = useState(false);
    const closeModal = () => setShowModal(false);
    const openModal = () => setShowModal(true);

    return (
        <div>
            <Modal show={showModal} onHide={closeModal} size="lg" centered>
                <Modal.Header closeButton>
                    <Modal.Title>My Profile</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Card>
                        <Card.Body>
                            <ListGroup variant="flush">
                                <ListGroup.Item>
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div className="fw-bold">Username / UPN</div>
                                        <div>{sessionData?.upn}</div>
                                    </div>
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div className="fw-bold">Name</div>
                                        <div>{sessionData?.name}</div>
                                    </div>
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div className="fw-bold">Subject (Sub)</div>
                                        <div>{sessionData?.sub}</div>
                                    </div>
                                </ListGroup.Item>
                                {sessionData?.roles?.map((role: string, index: number) => (
                                    <ListGroup.Item key={index}>
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div className="fw-bold">Role</div>
                                            <div>{role}</div>
                                        </div>
                                    </ListGroup.Item>
                                ))}
                                <ListGroup.Item>
                                    <div className="d-flex justify-content-between align-items-center">
                                        <div className="fw-bold">Session Expires At</div>
                                        <div>
                                            {sessionData?.expiresAt
                                                ? moment(sessionData.expiresAt).format("DD/MM/YYYY HH:mm:ss")
                                                : ""}
                                        </div>
                                    </div>
                                </ListGroup.Item>
                            </ListGroup>
                        </Card.Body>
                    </Card>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="danger" onClick={closeModal}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>

            <NavDropdown.Item onClick={openModal}>My Profile</NavDropdown.Item>
        </div>
    );
};