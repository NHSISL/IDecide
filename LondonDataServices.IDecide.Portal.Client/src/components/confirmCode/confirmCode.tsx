import React, { useState } from "react";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { Row, Col, Alert } from "react-bootstrap";
import { ConfirmCodeRequest } from "../../models/patients/confirmCodeRequest";
import { useStep } from "../../hooks/useStep";

interface ConfirmCodeProps {
    createdPatient: Patient | null;
}

export const ConfirmCode: React.FC<ConfirmCodeProps> = ({ createdPatient }) => {
    const [code, setCode] = useState("");
    const [error, setError] = useState("");
    const { nextStep, powerOfAttourney } = useStep();
    const confirmCodeMutation = patientViewService.useConfirmCode();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 5);
        setCode(value);
        // Do NOT clear error here; only clear on submit
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setError(""); // Clear previous error on submit
        if (code.length !== 5) {
            setError("Enter the 5 digit code sent to you");
            return;
        }
        if (!createdPatient) {
            setError("No patient found. Please restart the process.");
            return;
        }

        confirmCodeMutation.mutate(
            { nhsNumber: createdPatient.nhsNumber, code } as ConfirmCodeRequest,
            {
                onSuccess: () => {
                    nextStep(undefined, undefined, createdPatient);
                },
                onError: (error: unknown) => {
                    if (error instanceof Error) {
                        setError("Invalid code. Please try again.");
                        window.console.error("Error confirming code:", error.message);
                    } else {
                        setError("An error occurred. Please try again.");
                        window.console.error("Error confirming code:", error);
                    }
                }
            }
        );
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
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

                    <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit} >
                        <label className="nhsuk-label" htmlFor="code">
                            Enter Code
                        </label>
                        <input
                            className="nhsuk-input"
                            id="code"
                            name="code"
                            type="text"
                            inputMode="numeric"
                            maxLength={5}
                            autoComplete="one-time-code"
                            value={code}
                            onChange={handleInputChange}
                            style={{ width: "100%", maxWidth: "200px" }}
                            aria-describedby={error ? "code-error" : undefined}
                            aria-invalid={!!error}
                        />
                        {error && (
                            <div
                                id="code-error"
                                className="nhsuk-error-message"
                                style={{ marginTop: "0.5rem" }}
                                role="alert"
                            >
                                {error}
                            </div>
                        )}
                        <br />

                        <button
                            className="nhsuk-button"
                            type="submit"
                            style={{ width: "70%", marginTop: "1.5rem" }}
                            disabled={confirmCodeMutation.isPending}
                        >
                            {confirmCodeMutation.isPending ? "Submitting..." : "Submit"}
                        </button>
                    </form>
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
                        <h3>How do I get my code?</h3>
                        <p>
                            A 5-digit code has been sent to you by one of the following methods:
                        </p>
                        <ul>
                            <li><strong>SMS</strong> (text message) to your mobile phone</li>
                            <li><strong>Email</strong> to your registered email address</li>
                            <li><strong>Letter</strong> to your home address (please allow up to 3 days for delivery)</li>
                        </ul>
                        <p>
                            Once you receive your code, please enter it in the box provided. This helps us confirm that the contact details you have provided match our records.
                        </p>
                        <p>
                            If you have not received your code, please check your spam or junk email folder, or allow extra time if you are waiting for a letter.
                        </p>
                        <h3>What if I enter the wrong code?</h3>
                        <p>
                            If you enter the wrong code 3 times, you will be prompted to call our helpdesk to complete your opt-in or opt-out request.
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default ConfirmCode;