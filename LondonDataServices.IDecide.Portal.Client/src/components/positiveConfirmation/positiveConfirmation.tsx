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
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    Power of Attorney Details
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>Name:</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>Relationship:</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
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
                <div
                    className="p-4 mb-4"
                    style={{
                        background: "#f4f8fb",
                        border: "1px solid #d1e3f0",
                        borderRadius: "8px",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                    }}
                >
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>Help & Guidance</h2>
                    <h3 className="mb-3" style={{ color: "#005eb8" }}>
                        Receiving Your Confirmation Code
                    </h3>
                    <p>
                        To keep your information secure, we need to send you a one-time code. This code helps us confirm your identity before you continue.
                    </p>
                    <p>
                        Please choose how you would like to receive your code: by Email, SMS, or Post (letter). Select the option that is most convenient and accessible for you.
                    </p>
                    <p>
                        If you cannot access any of these methods, please contact one of our agents on ........
                    </p>
                    <p>
                        Your details are only used for identification and will not be shared outside the NHS.
                    </p>
                </div>
            </Col>
        </Row>
    );
};

export default PositiveConfirmation;