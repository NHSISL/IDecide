import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Row, Col, Alert } from "react-bootstrap";
import { Patient } from "../../models/patients/patient";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { patientViewService } from "../../services/views/patientViewService";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

const VALID_CODE_MESSAGE =
    "A valid code already exists for this patient, please go to the enter code screen.";

interface ConfirmDetailsProps {
    createdPatient: Patient;
    powerOfAttorney?: PowerOfAttourney;
}

const PositiveConfirmation = ({ createdPatient, powerOfAttorney }: ConfirmDetailsProps) => {
    const { t: translate } = useTranslation();
    const navigate = useNavigate();
    const [error, setError] = useState<string | JSX.Element>("");
    const [info, setInfo] = useState<string | JSX.Element>("");
    const [hideButtons, setHideButtons] = useState(false);
    const [resend, setResend] = useState(false);
    const [showResendMessage, setShowResendMessage] = useState(false);
    const { configuration } = useFrontendConfiguration();
    const updatePatient = patientViewService.useAddPatientAndGenerateCode();

    const handleSubmit = (method: "Email" | "Sms" | "Letter", resendFlag = false) => {
        setError("");
        setInfo("");

        const patientToUpdate = new PatientCodeRequest({
            nhsNumber: createdPatient.nhsNumber!,
            verificationCode: createdPatient.validationCode!,
            notificationPreference: method,
            generateNewCode: resendFlag ? true : false
        });

        updatePatient.mutate(
            patientToUpdate,
            {
                onSuccess: () => {
                    setError("");
                    setInfo("");
                    createdPatient.validationCode = patientToUpdate.verificationCode;
                    navigate("/confirmCode", { state: { createdPatient, powerOfAttorney } });
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
                        if (apiTitle === VALID_CODE_MESSAGE) {
                            createdPatient.validationCode = patientToUpdate.verificationCode;
                            setInfo(
                                <span>
                                    {apiTitle}{" Click "}
                                    <a
                                        href="#"
                                        onClick={e => {
                                            e.preventDefault();
                                            setError("");
                                            setInfo("");
                                            navigate("/confirmCode", { state: { createdPatient, powerOfAttorney } });
                                        }}
                                        style={{ textDecoration: "underline", color: "#005eb8" }}
                                    >
                                        {"Here"}
                                    </a>
                                </span>
                            );
                            setHideButtons(true);
                            setShowResendMessage(true);
                            return;
                        }
                        setError(apiTitle);
                        setShowResendMessage(false);
                        console.error("API Error updating patient:", apiTitle, errResponse);
                    } else if (
                        error &&
                        typeof error === "object" &&
                        "message" in error &&
                        typeof (error as { message?: unknown }).message === "string"
                    ) {
                        setError((error as { message: string }).message);
                        setShowResendMessage(false);
                        console.error("Error updating patient:", (error as { message: string }).message, error);
                    } else {
                        setError("An unexpected error occurred.");
                        setShowResendMessage(false);
                        console.error("Error updating patient:", error);
                    }
                }
            }
        );
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
                            <dd className="nhsuk-summary-list__value">{createdPatient.givenName + ',' + createdPatient.surname}</dd>
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

                    {showResendMessage && (
                        <Alert variant="danger">
                            <div id="code-error">
                                {translate("PositiveConfirmation.noCodeRecieved")}{" "}
                                <a
                                    href="#"
                                    onClick={e => {
                                        e.preventDefault();
                                        setHideButtons(false);
                                        setResend(true);
                                        setShowResendMessage(false);
                                        setInfo("");
                                    }}
                                    style={{ textDecoration: "underline", color: "#005eb8" }}>
                                    Here
                                </a>
                            </div>
                        </Alert>
                    )}

                    {!hideButtons && (
                        <div style={{
                            display: "flex",
                            gap: "1rem",
                            marginBottom: "0.5rem",
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
                        &nbsp;
                        <a
                            href={`tel:${configuration.helpdeskContactNumber}`}
                            style={{ textDecoration: "underline" }}
                        >
                            {configuration.helpdeskContactNumber}
                        </a> or email us at&nbsp;

                        <a
                            href={`mailto:${configuration.helpdeskContactEmail}`}
                            style={{ textDecoration: "underline" }}
                        >
                            {configuration.helpdeskContactEmail}
                        </a>.
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