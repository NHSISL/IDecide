import React, { useEffect } from "react";
import { useStep } from "../../hooks/useStep";
import { Row, Col, Alert } from "react-bootstrap";

interface ConfirmDetailsProps {
    goToConfirmCode: () => void;
}

export const ConfirmDetails: React.FC<ConfirmDetailsProps> = ({ goToConfirmCode }) => {
    const { setCurrentStepIndex, nextStep, createdPatient, powerOfAttourney } = useStep();

    useEffect(() => {
    }, [powerOfAttourney]);

    const handleNoClick = () => {
        setCurrentStepIndex(0);
    };

    const handleYesClick = () => {
        nextStep(undefined, undefined, createdPatient!, powerOfAttourney!);
    };

    if (!createdPatient) {
        return <div>No patient details available.</div>;
    }

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={6} lg={6}>
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
                    <h4 style={{ fontWeight: 700, fontSize: "1.5rem", margin: "1.5rem 0 1rem 0", color: "#212529" }}>Is this you?</h4>
                    <form className="nhsuk-form-group">
                        <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                            <div className="nhsuk-summary-list__row">
                                <dt className="nhsuk-summary-list__key">Name</dt>
                                <dd className="nhsuk-summary-list__value">{createdPatient.firstName + ',' + createdPatient.surname}</dd>
                            </div>
                            <div className="nhsuk-summary-list__row">
                                <dt className="nhsuk-summary-list__key">Email</dt>
                                <dd className="nhsuk-summary-list__value">{createdPatient.emailAddress}</dd>
                            </div>
                            <div className="nhsuk-summary-list__row">
                                <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                                <dd className="nhsuk-summary-list__value">07******084</dd>
                            </div>
                            <div className="nhsuk-summary-list__row">
                                <dt className="nhsuk-summary-list__key">Address</dt>
                                <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                            </div>
                        </dl>

                        <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem" }}>
                            <button
                                type="button"
                                className="nhsuk-button"
                                style={{ flex: 1 }}
                                onClick={handleYesClick}
                            >
                                Yes
                            </button>
                            <button
                                type="button"
                                className="nhsuk-button nhsuk-button--secondary"
                                style={{ flex: 1 }}
                                onClick={handleNoClick}
                            >
                                No
                            </button>
                        </div>

                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ width: "100%", marginBottom: 0, borderBottom: "none" }}
                            onClick={goToConfirmCode}
                        >
                            We have already been sent a code. Would you like to use that code to continue?
                        </button>
                    </form>
                </div>
            </Col>
            <Col xs={12} md={6} lg={6} className="custom-col-spacing">
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
                        Checking Your Details
                    </h3>
                    <p>
                        Please review the details shown on this page. Some information, such as your mobile number, may be partially hidden for your privacy.
                    </p>
                    <p>
                        If these details are correct and belong to you, click <strong>Yes</strong> to continue.
                    </p>
                    <p>
                        If these details do not match you, click <strong>No</strong> to return and try searching again with your NHS Number.
                    </p>
                    <p>
                        If you have already received a code by SMS, email, or post, you can use it now by clicking the button below to go straight to the code confirmation page.
                    </p>
                    <p>
                        This process helps us keep your information secure and ensures only you can access your records.
                    </p>
                </div>

            </Col>
        </Row>
    );
};

export default ConfirmDetails;