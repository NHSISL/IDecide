import React from "react";
import { useStep } from "../../hooks/useStep";
import { patientViewService } from "../../services/views/patientViewService";
import { GenerateCodeRequest } from "../../models/patients/generateCodeRequest";
import { Row, Col, Alert } from "react-bootstrap";

interface PositiveConfirmationProps {
    goToConfirmCode: (createdPatient: GenerateCodeRequest) => void;
}

const PositiveConfirmation: React.FC<PositiveConfirmationProps> = ({ goToConfirmCode }) => {
    const { createdPatient, powerOfAttourney } = useStep();
    const updatePatient = patientViewService.useUpdatePatient();

    if (!createdPatient) {
        return <div>No patient details available.</div>;
    }

    const patientToUpdate = new GenerateCodeRequest(createdPatient);

    const handleSubmit = (method: "Email" | "SMS" | "Letter") => {
        patientToUpdate.notificationPreference = method;

        if (powerOfAttourney) {
            patientToUpdate.poaFirstName = powerOfAttourney.firstName;
            patientToUpdate.poaSurname = powerOfAttourney.surname;
            patientToUpdate.poaRelationship = powerOfAttourney.relationship;
        }

        updatePatient.mutate(patientToUpdate, {
            onSuccess: () => {
                goToConfirmCode(patientToUpdate);
            },
            onError: (error: unknown) => {
                if (error instanceof Error) {
                    console.error("Error updating patient:", error.message);
                } else {
                    console.error("Error updating patient:", error);
                }
            }
        });
    };

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={7} lg={7}>
                <div className="mt-4">

                    {powerOfAttourney && (
                        <Alert variant="info" style={{ marginBottom: "1rem" }}>
                            <strong>Power of Attorney Details:</strong><br />
                            Name: <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>,&nbsp;
                            Relationship: <strong>{powerOfAttourney.relationship}</strong>
                        </Alert>
                    )}

                    <h2>Confirmation required</h2>
                    <p>Please confirm these details are correct before continuing:</p>
                    <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">Name</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.surname}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">Email</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.emailAddress}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.phoneNumber}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">Address</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                        </div>
                    </dl>

                    <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                        We need to send a code to you, how would you like to receive it:
                    </p>
                    <div style={{
                        display: "flex",
                        gap: "1rem",
                        marginBottom: "2rem",
                        flexWrap: "wrap"
                    }}>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("Email")}
                            disabled={!createdPatient.emailAddress}
                        >
                            Email
                        </button>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("SMS")}
                            disabled={!createdPatient.phoneNumber}
                        >
                            SMS
                        </button>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("Letter")}
                            disabled={!createdPatient.address}
                        >
                            Letter
                        </button>
                    </div>
                </div>
            </Col>
            <Col xs={12} md={5} lg={5} className="custom-col-spacing">
            </Col>
        </Row>
    );
};

export default PositiveConfirmation;