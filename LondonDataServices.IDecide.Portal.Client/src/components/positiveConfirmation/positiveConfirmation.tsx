import React, { useState } from "react";
import { useStep } from "../../hooks/useStep";
import { patientViewService } from "../../services/views/patientViewService";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { NotificationPreference } from "../../helpers/notificationPreference";

interface PositiveConfirmationProps {
    goToConfirmCode: (createdPatient: PatientCodeRequest) => void;
}

const notificationPreferenceMap: Record<"Email" | "Letter" | "Sms", number> = {
    Email: NotificationPreference.Email,
    Letter: NotificationPreference.Letter,
    Sms: NotificationPreference.Sms
};

const PositiveConfirmation: React.FC<PositiveConfirmationProps> = ({ goToConfirmCode }) => {
    const { t: translate } = useTranslation();
    const { createdPatient, powerOfAttorney, setCreatedPatient } = useStep();
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const updatePatient = patientViewService.useAddPatientAndGenerateCode();
    const [error, setError] = useState<string | JSX.Element>("");
    const [info, setInfo] = useState<string | JSX.Element>("");
    const [hideButtons, setHideButtons] = useState(false);
    const [resend, setResend] = useState(false);
    const [showAreYouSure, setShowAreYouSure] = useState(false);

    if (!createdPatient) {
        return <div>{translate("PositiveConfirmation.noPatientDetails")}</div>;
    }

    const handleSubmit = async (method: "Email" | "Letter" | "Sms", resendFlag = false) => {
        setError("");
        setInfo("");

        const patientToUpdate = new PatientCodeRequest({
            nhsNumber: createdPatient.nhsNumber!,
            verificationCode: createdPatient.validationCode!,
            notificationPreference: method,
            generateNewCode: resendFlag ? true : false
        });

        if (setCreatedPatient) {
            setCreatedPatient({
                ...createdPatient,
                notificationPreference: notificationPreferenceMap[method]
            });
        }

        await loadRecaptchaScript(RECAPTCHA_SITE_KEY);

        try {
            const token = await grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: RECAPTCHA_ACTION_SUBMIT });

            updatePatient.mutate(
                patientToUpdate,
                {
                    headers: { "X-Recaptcha-Token": token },
                    onSuccess: () => {
                        setError("");
                        setInfo("");
                        setResend(false);
                        goToConfirmCode(patientToUpdate);
                    },
                    onError: (error: unknown) => {
                        let apiTitle = "";
                        if (isApiErrorResponse(error)) {
                            const errResponse = error.response;
                            apiTitle =
                                errResponse.data?.title ||
                                errResponse.data?.message ||
                                errResponse.statusText ||
                                "Unknown API error";
                            setError(apiTitle);
                            console.error("API Error updating patient:", apiTitle, errResponse);
                        } else if (
                            error &&
                            typeof error === "object" &&
                            "message" in error &&
                            typeof (error as { message?: unknown }).message === "string"
                        ) {
                            setError((error as { message: string }).message);
                            console.error("Error updating patient:", (error as { message: string }).message, error);
                        } else {
                            setError("An unexpected error occurred.");
                        }
                    }
                }
            );
        } catch (err) {
            setError("Error executing reCAPTCHA.");
            console.error("Error executing reCAPTCHA:", err);
        }
    };

    const handleRequestNewCodeClick = () => {
        setShowAreYouSure(true);
        setHideButtons(true);
    };

    const handleAreYouSureNo = () => {
        setShowAreYouSure(false);
        setHideButtons(false);
    };

    const handleAreYouSureYes = () => {
        setShowAreYouSure(false);
        setHideButtons(false);
        setResend(true);
    };

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={7} lg={7}>
                <div className="mt-4">

                    {powerOfAttorney && (
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    {translate("PositiveConfirmation.poaDetailsTitle")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("PositiveConfirmation.poaNameLabel")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttorney.firstName} {powerOfAttorney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("PositiveConfirmation.poaRelationshipLabel")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttorney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}

                    <h2>{translate("PositiveConfirmation.confirmationRequiredTitle")}</h2>
                    <p>{translate("PositiveConfirmation.confirmationRequiredDescription")}</p>
                    <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryName")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.surname}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryEmail")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.email}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryMobile")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.phone}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryAddress")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                        </div>
                    </dl>

                    <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                        {translate("PositiveConfirmation.chooseMethod")}
                    </p>

                    {error && (
                        <Alert variant="danger">
                            <div id="code-error">{error}</div>
                        </Alert>
                    )}

                    {info && (
                        <Alert variant="info">
                            <div id="code-info">{info}</div>
                        </Alert>
                    )}

                    {!hideButtons && (
                        <>
                            <div style={{
                                display: "flex",
                                gap: "1rem",
                                flexWrap: "wrap"
                            }}>
                                <button
                                    type="button"
                                    className="nhsuk-button"
                                    style={{ flex: 1, minWidth: 120 }}
                                    onClick={() => handleSubmit("Email", resend)}
                                    disabled={!createdPatient.email}
                                >
                                    {translate("PositiveConfirmation.methodEmail")}
                                </button>
                                <button
                                    type="button"
                                    className="nhsuk-button"
                                    style={{ flex: 1, minWidth: 120 }}
                                    onClick={() => handleSubmit("Sms", resend)}
                                    disabled={!createdPatient.phone}
                                >
                                    {translate("PositiveConfirmation.methodSMS")}
                                </button>
                                <button
                                    type="button"
                                    className="nhsuk-button"
                                    style={{ flex: 1, minWidth: 120 }}
                                    onClick={() => handleSubmit("Letter", resend)}
                                    disabled={!createdPatient.address}
                                >
                                    {translate("PositiveConfirmation.methodLetter")}
                                </button>
                            </div>
                            <Alert variant="warning">
                            <p>If you have already requested a code but havent recieved it please click here to resend yourself a code</p>
                                <button
                                    type="button"
                                    className="nhsuk-button nhsuk-button--reverse"
                                    style={{ flex: 1, minWidth: 225 }}
                                    onClick={handleRequestNewCodeClick}
                                >
                                    {translate("PositiveConfirmation.requestNewCode") || "Request New Code"}
                                </button>
                            </Alert>
                        </>
                    )}

                    {showAreYouSure && (
                        <div style={{ marginTop: "1.5rem" }}>
                            <Alert variant="warning">
                                <div style={{ marginBottom: "1rem" }}>
                                    {translate("PositiveConfirmation.confirmNewCodeMessage") ||
                                        "Are you sure you want to generate a new code? This will invalidate your previous code."}
                                </div>
                                <div style={{ display: "flex", gap: "1rem" }}>
                                    <button
                                        type="button"
                                        className="nhsuk-button"
                                        onClick={handleAreYouSureYes}
                                    >
                                        {translate("PositiveConfirmation.confirm") || "Yes"}
                                    </button>
                                    <button
                                        type="button"
                                        className="nhsuk-button nhsuk-button--secondary"
                                        onClick={handleAreYouSureNo}
                                    >
                                        {translate("PositiveConfirmation.cancel") || "No"}
                                    </button>
                                </div>
                            </Alert>
                        </div>
                    )}
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
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("PositiveConfirmation.helpGuidanceTitle")}</h2>
                    <h3 className="mb-3" style={{ color: "#005eb8" }}>
                        {translate("PositiveConfirmation.helpReceivingCodeTitle")}
                    </h3>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription1")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription2")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription3")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription4")}
                    </p>
                </div>
            </Col>
        </Row>
    );
};

export default PositiveConfirmation;