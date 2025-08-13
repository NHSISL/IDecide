import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { Row, Col, Alert } from "react-bootstrap";
import { ConfirmCodeRequest } from "../../models/patients/confirmCodeRequest";
import { useStep } from "../../hooks/useStep";

interface ConfirmCodeProps {
    createdPatient: Patient | null;
}

export const ConfirmCode: React.FC<ConfirmCodeProps> = ({ createdPatient }) => {
    const { t } = useTranslation();
    const [code, setCode] = useState("");
    const [error, setError] = useState("");
    const { nextStep, powerOfAttourney } = useStep();
    const confirmCodeMutation = patientViewService.useConfirmCode();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 5);
        setCode(value);
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        if (code.length !== 5) {
            setError(t("ConfirmCode.errorEnterCode"));
            return;
        }
        if (!createdPatient) {
            setError(t("ConfirmCode.errorNoPatient"));
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
                        setError(t("ConfirmCode.errorInvalidCode"));
                        window.console.error("Error confirming code:", error.message);
                    } else {
                        setError(t("ConfirmCode.errorGeneric"));
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
                                    {t("ConfirmCode.powerOfAttorneyDetails")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmCode.powerOfAttorneyName")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmCode.powerOfAttorneyRelationship")}</dt>
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
                            {t("ConfirmCode.enterCodeLabel")}
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
                            {confirmCodeMutation.isPending ? t("ConfirmCode.submittingButton") : t("ConfirmCode.submitButton")}
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
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>{t("ConfirmCode.helpGuidanceTitle")}</h2>
                        <h3>{t("ConfirmCode.helpHowGetCodeTitle")}</h3>
                        <p>
                            {t("ConfirmCode.helpHowGetCodeText1")}
                        </p>
                        <ul>
                            <li><strong>{t("ConfirmCode.helpHowGetCodeSMS")}</strong></li>
                            <li><strong>{t("ConfirmCode.helpHowGetCodeEmail")}</strong></li>
                            <li><strong>{t("ConfirmCode.helpHowGetCodeLetter")}</strong></li>
                        </ul>
                        <p>
                            {t("ConfirmCode.helpHowGetCodeText2")}
                        </p>
                        <p>
                            {t("ConfirmCode.helpHowGetCodeText3")}
                        </p>
                        <h3>{t("ConfirmCode.helpWrongCodeTitle")}</h3>
                        <p>
                            {t("ConfirmCode.helpWrongCodeText")}
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default ConfirmCode;