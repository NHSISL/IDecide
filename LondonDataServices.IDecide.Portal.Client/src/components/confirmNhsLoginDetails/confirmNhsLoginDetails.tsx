import { useEffect, useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { Patient } from "../../models/patients/patient";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";
import { patientViewService } from "../../services/views/patientViewService";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";


export const ConfirmNhsLoginDetails: React.FC = () => {
    const { t: translate } = useTranslation();
    const { nextStep, createdPatient, setCreatedPatient, powerOfAttorney } = useStep();
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const updatePatient = patientViewService.useAddPatientNhsLogin();
    const [apiError, setApiError] = useState<string | JSX.Element>("");
    const [info, setInfo] = useState<string | JSX.Element>("");
    const { data: nhsLoginPatient, isSuccess } = patientViewService.useRetrievePatientInfoNhsLogin();

    const handleApiError = useApiErrorHandlerChecks({
        setApiError,
        configuration
    });

    useEffect(() => {
        if (isSuccess && nhsLoginPatient) {
            setCreatedPatient(
                new Patient({
                    nhsNumber: nhsLoginPatient.nhsNumber,
                    givenName: nhsLoginPatient.givenName,
                    surname: nhsLoginPatient.surname,
                    dateOfBirth: nhsLoginPatient.dateOfBirth
                        ? new Date(nhsLoginPatient.dateOfBirth)
                        : undefined,
                    email: nhsLoginPatient.email,
                    phone: nhsLoginPatient.phone
                })
            );
        }
    }, [isSuccess, nhsLoginPatient, setCreatedPatient]);

    const handleSubmit = async () => {
        setApiError("");
        setInfo("");

        const phoneNumber = createdPatient!.phone;

        try {
            await loadRecaptchaScript(RECAPTCHA_SITE_KEY);

            if (!window.grecaptcha || !window.grecaptcha.execute) {
                setApiError("reCAPTCHA failed to load. Please try again later.");
                return;
            }

            const token = await window.grecaptcha!.execute(
                RECAPTCHA_SITE_KEY,
                { action: RECAPTCHA_ACTION_SUBMIT }
            );

            updatePatient.mutate(
                phoneNumber!,
                {
                    headers: { "X-Recaptcha-Token": token },
                    onSuccess: () => {
                        setApiError("");
                        setInfo("");
                        nextStep(undefined, undefined, createdPatient!, powerOfAttorney!);
                    },
                    onError: (error: unknown) => {
                        if (isApiErrorResponse(error)) {
                            const errorResponse = error?.response;

                            const errorTitle =
                                errorResponse?.data?.title ||
                                errorResponse?.data?.message ||
                                errorResponse.statusText ||
                                "Unknown API error";

                            if (!handleApiError(errorTitle)) {
                                setApiError(errorTitle);
                            }
                            setApiError(errorTitle);
                        } else if (
                            error &&
                            typeof error === "object" &&
                            "message" in error &&
                            typeof (error as { message?: unknown }).message === "string"
                        ) {
                            setApiError((error as { message: string }).message);
                            console.error("Error updating patient:", (error as { message: string }).message, error);
                        } else {
                            setApiError("An unexpected error occurred.");
                        }
                    }
                }
            );
        } catch (err) {
            setApiError("Error executing reCAPTCHA.");
            console.error("Error executing reCAPTCHA:", err);
        }
    };

    const handleYesClick = () => {
        handleSubmit()
    };

    if (!createdPatient) {
        return <div>{translate("ConfirmDetails.noPatientDetails")}</div>;
    }

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={6} lg={6}>
                <div>
                    <h4 style={{ fontWeight: 700, fontSize: "1.5rem", margin: "0 0 1rem 0", color: "#212529" }}>
                        {translate("ConfirmDetails.confirmDetails", "Confirm your details")}
                    </h4>
                    <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.name")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.givenName + ',' + createdPatient.surname}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.email")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.email}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.mobileNumber")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.phone}</dd>
                        </div>
                    </dl>

                    <div style={{ display: "flex", marginBottom: "0.2rem", width: "25%" }}>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1 }}
                            onClick={handleYesClick}
                        >
                            {translate("ConfirmDetails.Continue", "Next")}
                        </button>
                    </div>
                    <Alert variant="info" style={{ marginTop: "0.5rem" }}>
                        <p>
                            <span style={{ marginRight: "0.5rem" }}>
                                <FontAwesomeIcon icon={faCircleInfo} style={{ color: "#005eb8" }} ></FontAwesomeIcon>
                            </span>

                            If these details are incorrect, you'll need to update your details at your GP practice. You can do this by contacting your GP directly.</p>
                        <p>
                            Once your details are updated, you'll be able to make your choice online.
                        </p>

                        <p>
                            If you do not know your GP's contact details or are not registered with one,
                            try using the <a href="https://www.nhs.uk/service-search/find-a-gp" target="_blank" rel="noopener noreferrer">
                                find a GP</a> service.
                        </p>

                        <p>
                            Click{' '}
                            <a
                                href="#"
                                style={{ padding: 0, border: "none", background: "none", color: "#005eb8", textDecoration: "underline" }}
                                onClick={e => {
                                    e.preventDefault();
                                    fetch('/logout', { method: 'POST' }).then(d => {
                                        if (d.ok) {
                                            window.location.href = '/';
                                        }
                                    });
                                }}
                            >
                                here
                            </a>{' '}
                            to logout.
                        </p>
                    </Alert>
                </div>

                {apiError && (
                    <Alert variant="danger">
                        <div id="code-error">{apiError}</div>
                    </Alert>
                )}

                {info && (
                    <Alert variant="info">
                        <div id="code-info">{info}</div>
                    </Alert>
                )}
            </Col>
            <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                <div
                    className="p-4 mb-4"
                    style={{
                        background: "#f4f8fb",
                        border: "1px solid #d1e3f0",
                        borderRadius: "8px",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                    }}>
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>
                        Help & Guidance
                    </h2>
                    <p>
                        Please review the details shown on this page. Some information, such as your mobile number, may be partially hidden for your privacy.
                    </p>
                    <p>
                        If these details are correct and belong to you, click Next to continue.
                    </p>
                    <p>
                        <strong>Why check your details?</strong> This helps ensure your information is accurate and up to date, which is important for your care and for keeping your records secure.
                    </p>
                    <p>
                        <strong>If your details are incorrect:</strong> You will need to update them with your GP practice before continuing. This helps protect your privacy and ensures you receive important information.
                    </p>
                    <p>
                        <strong>How your information is used:</strong> The details you confirm here will only be used to verify your identity and provide you with the right services..
                    </p>
                    <p>
                        <strong>Security reminder:</strong> Never share your NHS login details or personal information with anyone. The NHS will never ask you for your password or security codes by email or phone.
                    </p>
                </div>
            </Col>
        </Row>
    );
};

export default ConfirmNhsLoginDetails;