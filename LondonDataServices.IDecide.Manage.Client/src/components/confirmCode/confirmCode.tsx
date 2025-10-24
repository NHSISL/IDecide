import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Patient } from "../../models/patients/patient";
import { useTranslation } from "react-i18next";
import { patientViewService } from "../../services/views/patientViewService";
import { Row, Col, Alert, Button, Spinner } from "react-bootstrap";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";

interface ConfirmDetailsProps {
    createdPatient: Patient;
    powerOfAttorney?: PowerOfAttourney;
}

export const ConfirmCode = ({ createdPatient, powerOfAttorney }: ConfirmDetailsProps) => {
    const { t: translate } = useTranslation();
    const navigate = useNavigate();
    const [code, setCode] = useState("");
    const [error, setError] = useState("");
    const { configuration } = useFrontendConfiguration();
    const confirmCodeMutation = patientViewService.useConfirmCode();

    const {
        mappedPatients: patientsByNhs,
        isLoading: isLoadingPatient
    } = patientViewService.useGetAllPatients(
        createdPatient?.nhsNumber ? { nhsNumber: createdPatient.nhsNumber } : undefined
    );

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/[^a-zA-Z0-9]/g, "").slice(0, 5);
        setCode(value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        if (code.length !== 5) {
            setError(translate("ConfirmCode.errorEnterCode"));
            return;
        }

        if (!createdPatient) {
            setError(translate("ConfirmCode.errorNoPatient"));
            return;
        }

        try {
            const request = new PatientCodeRequest({
                nhsNumber: createdPatient.nhsNumber!,
                verificationCode: code,
                notificationPreference: "",
                generateNewCode: false
            });

            await confirmCodeMutation.mutateAsync(request);

            createdPatient.validationCode = code;
            navigate("/optInOut", { state: { createdPatient, powerOfAttorney } });
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
                        <br/>
                        <small style={{ fontSize: "12px" }}>
                            <strong style={{ fontSize: "12px" }}>Note</strong>: this is case sensitive.
                        </small>

                        {error && (
                            <div
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
                            </div>
                        )}
                        <br />
                        <button
                            className="nhsuk-button"
                            type="submit"
                            style={{ width: "70%", marginTop: "0.2rem" }}
                            disabled={confirmCodeMutation.isPending}
                        >
                            {confirmCodeMutation.isPending
                                ? translate("ConfirmCode.submittingButton")
                                : translate("ConfirmCode.submitButton")}
                        </button>

                        {patientsByNhs && patientsByNhs.length === 1 && (
                            <>
                                <Alert variant="danger">
                                    <p>
                                        The current Verification Code for this patient is:{" "}
                                        {isLoadingPatient ? (
                                            <Spinner
                                                animation="border"
                                                size="sm"
                                                role="status"
                                                aria-hidden="true"
                                                style={{ marginLeft: "0.5rem", verticalAlign: "middle" }}
                                            />
                                        ) : (
                                            <strong>{patientsByNhs[0].validationCode}</strong>
                                        )}
                                    </p>
                                    <p>
                                        Please ask the patient to read out their verification code over the phone. Confirm that the code
                                        they provide matches the code shown above before proceeding.
                                    </p>
                                    <p>
                                        As the agent, once you have confirmed the patient's identity and the code matches, you may use
                                        this code for verification by clicking the button below.
                                    </p>
                                    <Button
                                        onClick={() => {
                                            if (patientsByNhs && patientsByNhs.length === 1) {
                                                setCode(patientsByNhs[0].validationCode || "");
                                            }
                                        }}
                                        className="ms-2"
                                    >
                                        Use Verification Code
                                    </Button>
                                </Alert>
                            </>
                        )}

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
