import { useMsal } from "@azure/msal-react";
import { ReactElement, useState } from "react";
import { Button, Card, ListGroup, Modal, NavDropdown } from "react-bootstrap";
import MyOdsAssigned from "../ods/myOdsAssigned";
import { UserAgreementModal } from "../userAgreements/userAgreements";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFileContract } from "@fortawesome/free-solid-svg-icons/faFileContract";

interface UserProfileProps {
    modalTitle?: string;
    className?: string;
}

export const UserProfile = ({ modalTitle = "My Profile", className }: UserProfileProps): ReactElement => {
    const { accounts } = useMsal();
    const [showModal, setShowModal] = useState(false);
    const closeModal = () => setShowModal(false);
    const openModal = () => setShowModal(true);
    const [showAgreement, setShowAgreement] = useState(false);

    return (
        accounts[0] && (
            <div className={className}>
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
                                            <div className="fw-bold">Username / Email</div>
                                            <div>{accounts[0]?.username}</div>
                                        </div>
                                    </ListGroup.Item>
                                    <ListGroup.Item>
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div className="fw-bold">Name</div>
                                            <div>{accounts[0]?.name}</div>
                                        </div>
                                    </ListGroup.Item>
                                    {accounts[0]?.idTokenClaims?.roles?.map((r, i) => (
                                        <ListGroup.Item key={i}>
                                            <div className="d-flex justify-content-between align-items-center">
                                                <div className="fw-bold">Roles</div>
                                                <div>{r}</div>
                                            </div>
                                        </ListGroup.Item>
                                    ))}
                                    <ListGroup.Item>
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div className="fw-bold">Organisations </div>
                                            <div style={{ maxWidth: "400px" }}><MyOdsAssigned /></div>
                                        </div>
                                    </ListGroup.Item>

                                </ListGroup>

                            </Card.Body>
                        </Card>
                    </Modal.Body>
                    <Modal.Footer className="d-flex justify-content-between align-items-center">
                        <div className="d-flex align-items-center">
                            <div className="fw-bold"></div>
                            <Button variant="primary" onClick={() => { setShowAgreement(true) }}>
                                <FontAwesomeIcon icon={faFileContract} className="me-2" />View Terms & Conditions
                            </Button>
                            {showAgreement &&
                                <UserAgreementModal viewOnly={true} hideModel={() => { setShowAgreement(false) }} />
                            }
                        </div>

                        <Button variant="danger" onClick={closeModal}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>

                <NavDropdown.Item onClick={openModal}>{modalTitle}</NavDropdown.Item>
            </div>
        )
    );
};