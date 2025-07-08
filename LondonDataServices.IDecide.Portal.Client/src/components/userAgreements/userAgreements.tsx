import React from "react";
import { Button, Modal } from "react-bootstrap";
import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";
import { userAgreementService } from "../../services/foundations/userAgreementService";
import { UserAgreement } from "../../models/userAgreement/userAgreement";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { useAgreementCatalogue } from "./agreementCatalogue";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";
import { faCheck } from "@fortawesome/free-solid-svg-icons/faCheck";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";

interface UserAgreementModalProps {
    viewOnly?: boolean;
    hideModel?: () => void;
}

export const UserAgreementModal: React.FC<UserAgreementModalProps> = ({ viewOnly = false, hideModel }) => {
    const { accounts, instance } = useMsal();
    const currentAgreementType = "UserAgreement";
    const { configuration } = useFrontendConfiguration();
    const { data: hasAgreed, isLoading } = userAgreementService.useRetrieveAgreement(accounts[0]?.localAccountId, configuration?.activeAgreement, currentAgreementType);
    const { mutate, isPending, isSuccess } = userAgreementService.useAcceptAgreement();
    const { getAgreement } = useAgreementCatalogue();

    function agree(): void {
        const newAgreement = {
            id: crypto.randomUUID(),
            UserId: accounts[0].localAccountId,
            agreementVersion: configuration?.activeAgreement,
            agreementType: currentAgreementType,
            agreementDate: new Date(),
            createdDate: new Date(),
            createdBy: accounts[0].username,
            updatedDate: new Date(),
            updatedBy: accounts[0].username
        } as UserAgreement;

        mutate(newAgreement);
    }

    if (configuration?.activeAgreement === undefined) {
        return <Modal>Configuration does not define user agreement.</Modal>;
    }

    return (
        <AuthenticatedTemplate>
            {isLoading || isSuccess ? <></> :
                <Modal show={(viewOnly || !hasAgreed)} dialogClassName="modal-90w">
                    <Modal.Header>
                        <Modal.Title>User Agreement - Version {configuration?.activeAgreement}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body className="scrollable-modal-body">
                        {!viewOnly && <p>You must accept the following user agreement before you can use this application.</p>}
                        {getAgreement(currentAgreementType, configuration.activeAgreement)}
                    </Modal.Body>
                    <Modal.Footer>
                        {viewOnly ? (
                            <Button variant="secondary" onClick={hideModel}><FontAwesomeIcon icon={faTimes} className="me-1" />Close</Button>
                        ) : (
                                <>
                                    <Button variant="outline-secondary" onClick={() => { instance.logoutRedirect() }}><FontAwesomeIcon icon={faTimes} className="me-1" />Decline and Logout</Button>
                                <Button variant="success" disabled={isPending} onClick={agree}>
                                    {isPending ? <SpinnerBase /> : <><FontAwesomeIcon icon={faCheck} className="me-1" /> Accept</>}
                                </Button>
                            </>
                        )}
                    </Modal.Footer>
                </Modal>
            }
        </AuthenticatedTemplate>
    );
};