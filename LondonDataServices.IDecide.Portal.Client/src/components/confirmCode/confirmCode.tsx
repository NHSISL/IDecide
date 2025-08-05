import React, { useState } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { Row, Col } from "react-bootstrap";
import { ConfirmCodeRequest } from "../../models/patients/confirmCodeRequest";

interface ConfirmCodeProps {
    createdPatient: Patient;
}

export const ConfirmCode: React.FC<ConfirmCodeProps> = ({ createdPatient }) => {
    const [code, setCode] = useState("12345");
    const [error, setError] = useState("");
    const { nextStep } = useStep();
    const confirmCodeMutation = patientViewService.useConfirmCode();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 5);
        setCode(value);
        if (error) setError("");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (code.length !== 5) {
            setError("Please enter a 5-digit code.");
            return;
        }

        confirmCodeMutation.mutate(
            { nhsNumber: createdPatient.nhsNumber, code } as ConfirmCodeRequest,
            {
                onSuccess: () => {
                    nextStep(undefined, undefined,createdPatient);
                },
                onError: (error: unknown) => {
                    if (error instanceof Error) {
                        setError("Invalid code. Please try again.");
                        console.error("Error confirming code:", error.message);
                    } else {
                        setError("An error occurred. Please try again.");
                        console.error("Error confirming code:", error);
                    }
                }
            }
        );
    };

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={7} lg={7}>
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
                        pattern="\d{5}"
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
                            <strong>Error:</strong> {error}
                        </div>
                    )}

                    <button
                        className="nhsuk-button"
                        type="submit"
                        style={{ width: "100%", marginTop: "1.5rem" }}
                        disabled={confirmCodeMutation.isLoading}
                    >
                        {confirmCodeMutation.isLoading ? "Submitting..." : "Submit"}
                    </button>
                </form>
            </Col>
            <Col xs={12} md={5} lg={5} className="custom-col-spacing">
            </Col>
        </Row>
    );
};

export default ConfirmCode;