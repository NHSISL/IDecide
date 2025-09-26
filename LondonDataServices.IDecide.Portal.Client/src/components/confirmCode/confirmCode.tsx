/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { Row, Col, Alert } from "react-bootstrap";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { useStep } from "../../hooks/useStep";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";

interface ConfirmCodeProps {
    createdPatient: Patient | null;
}

export const ConfirmCode: React.FC<ConfirmCodeProps> = ({ createdPatient }) => {
    const { t: translate } = useTranslation();
    const [code, setCode] = useState("");
    const [error, setError] = useState("");
    const [info, setInfo] = useState("");
    const [resendPending, setResendPending] = useState(false);
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const { nextStep, powerOfAttorney } = useStep();
    const confirmCodeMutation = patientViewService.useConfirmCode();
    const resendCodeMutation = patientViewService.useAddPatientAndGenerateCode();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/[^a-zA-Z0-9]/g, "").slice(0, 5);
        setCode(value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setInfo("");

        if (code.length !== 5) {
            setError(translate("ConfirmCode.errorEnterCode"));
            return;
        }

        if (!createdPatient) {
            setError(translate("ConfirmCode.errorNoPatient"));
            return;
        }

        try {
            await loadRecaptchaScript(RECAPTCHA_SITE_KEY);

            if (!(window as any).grecaptcha) {
                setError("reCAPTCHA is not loaded. Please try again later.");
                return;
            }

            const token = await (window as any).grecaptcha.execute(
                RECAPTCHA_SITE_KEY,
                { action: RECAPTCHA_ACTION_SUBMIT }
            );

            const request = new PatientCodeRequest({
                nhsNumber: createdPatient.nhsNumber!,
                verificationCode: code,
                notificationPreference: "",
                generateNewCode: false
            });

            await confirmCodeMutation.mutate(request, {
                headers: { "X-Recaptcha-Token": token }
            });
            createdPatient.validationCode = code;
            nextStep(undefined, undefined, createdPatient);
        } catch (error: unknown) {
            if (isApiErrorResponse(error)) {
                const errResponse = error.response;
                const apiTitle =
                    errResponse.data?.title ||
                    errResponse.data?.message ||
                    errResponse.statusText ||
                    "Unknown API error";
                setError(apiTitle);
            } else if (
                error &&
                typeof error === "object" &&
                "message" in error &&
                typeof (error as { message?: unknown }).message === "string"
            ) {
                setError(translate("ConfirmCode.errorInvalidCode"));
            } else {
                setError(translate("ConfirmCode.errorGeneric"));
            }
        }
    };

    const getPatientToUpdate = () => {
        if (
            !createdPatient ||
            !createdPatient.nhsNumber
        ) {
            return null;
        }
        return new PatientCodeRequest({
            nhsNumber: createdPatient.nhsNumber,
            notificationPreference:
                createdPatient.notificationPreference != null
                    ? String(createdPatient.notificationPreference)
                    : "",
            verificationCode: "",
            generateNewCode: true
        });
    };

    const handleResendCode = async () => {
        setResendPending(true);
        setError("");
        setInfo("");

        const patientToUpdate = getPatientToUpdate();
        if (!patientToUpdate) {
            setError("Patient information is missing.");
            setResendPending(false);
            return;
        }

        try {
            await loadRecaptchaScript(RECAPTCHA_SITE_KEY);

            if (!(window as any).grecaptcha) {
                setError("reCAPTCHA is not loaded. Please try again later.");
                setResendPending(false);
                return;
            }

            const token = await (window as any).grecaptcha.execute(
                RECAPTCHA_SITE_KEY,
                { action: RECAPTCHA_ACTION_SUBMIT }
            );

            resendCodeMutation.mutate(
                patientToUpdate,
                {
                    headers: { "X-Recaptcha-Token": token },
                    onSuccess: () => {
                        setError("");
                        setInfo("A new code has been sent.");
                        setResendPending(false);
                    },
                    onError: (error: unknown) => {
                        let apiTitle = "";
                        setError("Failed to resend code. Please try again or call the helpdesk.");
                        if (isApiErrorResponse(error)) {
                            const errResponse = error.response;
                            apiTitle =
                                errResponse.data?.title ||
                                errResponse.data?.message ||
                                errResponse.statusText ||
                                "Unknown API error";
                            if (
                                apiTitle ===
                                "A valid code already exists for this patient, please go to the enter code screen."
                            ) {
                                setResendPending(false);
                                return;
                            }
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
                            console.error("Error updating patient:", error);
                        }
                        setResendPending(false);
                    }
                }
            );
        } catch (err) {
            setError("Error executing reCAPTCHA.");
            setResendPending(false);
            console.error("Error executing reCAPTCHA:", err);
        }
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
                    {powerOfAttorney && (
                        <Alert
                            variant="info"
                            className="d-flex align-items-center"
                            style={{ marginBottom: "0.75rem", padding: "0.75rem" }}
                        >
                            <div
                                className="me-2"
                                style={{ fontSize: "1.5rem", color: "#6c757d" }}
                            />
                            <div>
                                <div
                                    style={{
                                        fontSize: "1rem",
                                        marginBottom: "0.25rem",
                                        color: "#6c757d",
                                        fontWeight: 500
                                    }}
                                >
                                    {translate("ConfirmCode.powerOfAttorneyDetails")}
                                </div>
                                <dl
                                    className="mb-0"
                                    style={{ fontSize: "0.95rem", color: "#6c757d" }}
                                >
                                    <div>
                                        <dt
                                            style={{
                                                display: "inline",
                                                fontWeight: 500
                                            }}
                                        >
                                            {translate("ConfirmCode.powerOfAttorneyName")}
                                        </dt>
                                        <dd
                                            style={{
                                                display: "inline",
                                                marginLeft: "0.5rem"
                                            }}
                                        >
                                            <strong>
                                                {powerOfAttorney.firstName} {powerOfAttorney.surname}
                                            </strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt
                                            style={{
                                                display: "inline",
                                                fontWeight: 500
                                            }}
                                        >
                                            {translate("ConfirmCode.powerOfAttorneyRelationship")}
                                        </dt>
                                        <dd
                                            style={{
                                                display: "inline",
                                                marginLeft: "0.5rem"
                                            }}
                                        >
                                            <strong>
                                                {powerOfAttorney.relationship}
                                            </strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}

                    <form
                        className="nhsuk-form-group"
                        autoComplete="off"
                        onSubmit={handleSubmit}
                    >
                        <label className="nhsuk-label" htmlFor="code">
                            {translate("ConfirmCode.enterCodeLabel")}
                        </label>
                        <input
                            className="nhsuk-input"
                            id="code"
                            name="code"
                            type="text"
                            maxLength={5}
                            autoComplete="one-time-code"
                            value={code}
                            onChange={handleInputChange}
                            style={{ width: "100%", maxWidth: "200px" }}
                            aria-describedby={error ? "code-error" : undefined}
                            aria-invalid={!!error}
                        />
                        {/* Info message section */}
                        {info && (
                            <Alert
                                className="nhsuk-info-message"
                                style={{
                                    marginTop: "0.5rem",
                                    color: "#005eb8",
                                    background: "#e6f4fa",
                                    padding: "0.75rem",
                                    borderRadius: "4px"
                                }}
                                role="status"
                            >
                                {info}
                            </Alert>
                        )}
                        {/* Error message section */}
                        {error && (
                            <Alert
                                variant="danger"
                                id="code-error"
                                className="nhsuk-error-message"
                                style={{ marginTop: "0.5rem" }}
                                role="alert"
                            >
                                {error}
                                {error === "The maximum retry count of 3 exceeded." && (
                                    <div style={{ marginTop: "0.5rem", fontWeight: "normal" }}>
                                        You have entered the code wrong 3 times, please call our helpdesk
                                        on{" "}
                                        <a
                                            href={`tel:${configuration.helpdeskContactNumber}`}
                                            style={{ textDecoration: "underline" }}
                                        >
                                            {configuration.helpdeskContactNumber}
                                        </a>{" "}
                                        to complete your opt-in or opt-out request,
                                        or alternatively email us at{" "}
                                        <a
                                            href={`mailto:${configuration.helpdeskContactEmail}`}
                                            style={{ textDecoration: "underline" }}
                                        >
                                            {configuration.helpdeskContactEmail}
                                        </a>
                                    </div>
                                )}
                            </Alert>
                        )}
                        <br />
                        <button
                            className="nhsuk-button"
                            type="submit"
                            style={{ width: "70%", marginTop: "0.2rem" }}
                        >
                            {translate("ConfirmCode.submitButton")}
                        </button>
                        <Alert>
                            <div style={{  fontSize: "0.95rem", color: "#333" }}>
                                I have not received a code {" "}
                                <span
                                    onClick={resendPending ? undefined : handleResendCode}
                                    style={{
                                        color: resendPending ? "#999" : "#005eb8",
                                        textDecoration: "underline",
                                        cursor: resendPending ? "not-allowed" : "pointer"
                                    }}
                                    aria-disabled={resendPending}
                                    tabIndex={resendPending ? -1 : 0}
                                    role="button"
                                    onKeyDown={e => {
                                        if (!resendPending && (e.key === "Enter" || e.key === " ")) {
                                            if (e.key === " ") {
                                                e.preventDefault();
                                            }
                                            handleResendCode();
                                        }
                                    }}
                                >
                                    click here to resend
                                </span>
                                &nbsp; alternatively, please call the helpdesk on&nbsp;
                                <a
                                    href={`tel:${configuration.helpdeskContactNumber}`}
                                    style={{ textDecoration: "underline" }}
                                >
                                    {configuration.helpdeskContactNumber}
                                </a>{" "}
                                &nbsp;or email&nbsp;
                                <a
                                    href={`mailto:${configuration.helpdeskContactEmail}`}
                                    style={{ textDecoration: "underline" }}
                                >
                                    {configuration.helpdeskContactEmail}
                                </a>
                                .
                            </div>
                        </Alert>
                    </form>
                </Col>
                <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                    <div
                        className="p-4 mb-4"
                        style={{
                            background: "#f4f8fb",
                            border: "1px solid #d1e3f0",
                            borderRadius: "8px",
                            boxShadow: "0 2px 8px rgba(0,0,0,0.04)"
                        }}
                    >
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>
                            {translate("ConfirmCode.helpGuidanceTitle")}
                        </h2>
                        <h3>{translate("ConfirmCode.helpHowGetCodeTitle")}</h3>
                        <p>
                            {translate("ConfirmCode.helpHowGetCodeText1")}
                        </p>
                        <ul>
                            <li>
                                <strong>
                                    {translate("ConfirmCode.helpHowGetCodeSMS")}
                                </strong>
                            </li>
                            <li>
                                <strong>
                                    {translate("ConfirmCode.helpHowGetCodeEmail")}
                                </strong>
                            </li>
                            <li>
                                <strong>
                                    {translate("ConfirmCode.helpHowGetCodeLetter")}
                                </strong>
                            </li>
                        </ul>
                        <p>
                            {translate("ConfirmCode.helpHowGetCodeText2")}
                        </p>
                        <p>
                            {translate("ConfirmCode.helpHowGetCodeText3")}
                        </p>
                        <h3>{translate("ConfirmCode.helpWrongCodeTitle")}</h3>
                        <p>
                            {translate("ConfirmCode.helpWrongCodeText")}
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default ConfirmCode;