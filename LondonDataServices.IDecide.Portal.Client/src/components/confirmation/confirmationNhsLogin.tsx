import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../../hooks/useStep";
import { decisionViewService } from "../../services/views/decisionViewService";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { useTranslation } from "react-i18next";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { Patient } from "../../models/patients/patient";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";
import { faArrowLeftLong } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface ConfirmationNhsLoginProps {
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
    createdPatient?: Patient | null;
}

export const ConfirmationNhsLogin: React.FC<ConfirmationNhsLoginProps> = ({
    selectedOption,
    nhsNumber,
    createdPatient
}) => {

    const { nextStep, previousStep } = useStep();
    const createDecisionMutation = decisionViewService.useCreatePatientDecisionNhsLogin();
    const [apiError, setApiError] = useState<string | JSX.Element>("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { t: translate } = useTranslation();
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const [notificationPreference, setNotificationPreference] = useState<"SMS" | "Email" | "None" | "">("");
    const EmptyGuid = "00000000-0000-0000-0000-000000000000"
    const handleApiError = useApiErrorHandlerChecks({
        setApiError,
        configuration
    });

    const handleNotificationChange = (value: "SMS" | "Email" | "None") => {
        setNotificationPreference(prev =>
            prev === value ? "" : value
        );
    };

    const handleBack = () => {
        previousStep();
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!notificationPreference) {
            setApiError("Please select how you would like to be notified.");
            return;
        }

        setApiError("");
        setIsSubmitting(true);

        const decision = new PatientDecision({
            id: crypto.randomUUID(),
            patientId: EmptyGuid,
            patient: {
                nhsNumber: nhsNumber || "",
                validationCode: "LOGIN",
                notificationPreference: notificationPreference === "Email"
                    ? 0
                    : notificationPreference === "SMS"
                        ? 2
                        : notificationPreference === "None"
                            ? 3
                            : undefined
            },
            decisionChoice: selectedOption!,
            decisionTypeId: configuration.decisionTypeId
        });

        try {
            const token = await window.grecaptcha!.execute(
                RECAPTCHA_SITE_KEY,
                { action: RECAPTCHA_ACTION_SUBMIT }
            );

            createDecisionMutation.mutate(
                decision,
                {
                    headers: { "X-Recaptcha-Token": token },
                    onSuccess: () => {
                        setIsSubmitting(false);
                        nextStep();
                    },
                    // eslint-disable-next-line @typescript-eslint/no-explicit-any
                    onError: (error: any) => {

                        const status = error?.response?.status;
                        const errorData = error?.response?.data;
                        const errorTitle = errorData?.title;

                        if (handleApiError(errorTitle)) {
                            setIsSubmitting(false);
                            return;
                        }

                        console.log(status);
                        handleApiError(errorTitle);
                        setIsSubmitting(false);

                        if (errorTitle === "Invalid decision reference error occurred.") {
                            setApiError("Decision Type not set in database.");
                            setIsSubmitting(false);
                            return;
                        }

                        switch (status) {
                            case 400:
                                setApiError(translate("errors.400"));
                                break;
                            case 404:
                                setApiError(translate("errors.404"));
                                break;
                            case 401:
                                setApiError(translate("errors.401"));
                                break;
                            case 500:
                                setApiError(translate("errors.500")
                                );
                                break;
                            default:
                                setApiError(
                                    errorTitle ||
                                    translate("errors.CatchAll")
                                );
                                break;
                        }
                    }
                });
        } catch (err) {
            setApiError("Error executing reCAPTCHA.");
            console.error("Error executing reCAPTCHA:", err);
        }
    };

    const getNotificationPreferenceLabel = () => {
        if (notificationPreference === "SMS") return "SMS";
        if (notificationPreference === "Email") return "Email";
        if (notificationPreference === "None") return "None";
        return <span style={{ color: "#888" }}>Not selected</span>;
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>

                    <div className="nhsuk-card nhsuk-card--summary">

                        <div className="nhsuk-card__content">
                            <h3 className="nhsuk-card__heading">Your Data Sharing Choice</h3>

                            <dl className="nhsuk-summary-list mb-2">
                                <div className="nhsuk-summary-list__row">
                                    <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>Decision</dt>
                                    <dd className="nhsuk-summary-list__value">
                                        <strong data-testid="decision-value">
                                            {selectedOption === "optin"
                                                ? translate("ConfirmAndSave.decisionOptIn")
                                                : selectedOption === "optout"
                                                    ? translate("ConfirmAndSave.decisionOptOut")
                                                    : translate("ConfirmAndSave.decisionNotSelected")}
                                        </strong>
                                    </dd>
                                </div>

                                <div className="nhsuk-summary-list__row">
                                    <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>Name</dt>
                                    <dd className="nhsuk-summary-list__value">
                                        <strong data-testid="nhs-number-value">
                                            {createdPatient?.givenName},{createdPatient?.surname}
                                        </strong>
                                    </dd>
                                </div>

                                <div className="nhsuk-summary-list__row">
                                    <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>
                                        Notification Preference
                                    </dt>
                                    <dd className="nhsuk-summary-list__value">
                                        <strong data-testid="notification-preference-value">
                                            {getNotificationPreferenceLabel()}
                                        </strong>
                                    </dd>
                                </div>
                            </dl>

                            <Alert>
                                {/* Notification Preference Selection */}
                                <div className="nhsuk-form-group" style={{ marginBottom: "1.5rem" }}>
                                    <fieldset className="nhsuk-fieldset">
                                        <legend className="nhsuk-fieldset__legend nhsuk-fieldset__legend--m">
                                            How would you like to be notified when your data flows into the London Data Service?
                                        </legend>
                                        <div className="nhsuk-checkboxes">
                                            <div className="nhsuk-checkboxes__item">
                                                <input
                                                    className="nhsuk-checkboxes__input"
                                                    id="notify-text"
                                                    name="notificationPreferenceText"
                                                    type="checkbox"
                                                    checked={notificationPreference === "SMS"}
                                                    onChange={() => handleNotificationChange("SMS")}
                                                />
                                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="notify-text">
                                                    SMS
                                                </label>
                                            </div>
                                            <div className="nhsuk-checkboxes__item">
                                                <input
                                                    className="nhsuk-checkboxes__input"
                                                    id="notify-email"
                                                    name="notificationPreferenceEmail"
                                                    type="checkbox"
                                                    checked={notificationPreference === "Email"}
                                                    onChange={() => handleNotificationChange("Email")}
                                                />
                                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="notify-email">
                                                    Email
                                                </label>
                                            </div>
                                            <div className="nhsuk-checkboxes__item">
                                                <input
                                                    className="nhsuk-checkboxes__input"
                                                    id="notify-none"
                                                    name="notificationPreferenceNone"
                                                    type="checkbox"
                                                    checked={notificationPreference === "None"}
                                                    onChange={() => handleNotificationChange("None")}
                                                />
                                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="notify-none">
                                                    I dont want to be notified.
                                                </label>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </Alert>

                            <hr />
                            <form className="nhsuk-form-group" onSubmit={handleSubmit} data-testid="confirmation-form" >
                                <button
                                    className="nhsuk-button"
                                    type="submit"
                                    style={{ width: "100%", marginBottom: "5px" }}
                                    data-testid="save-preferences-btn"
                                    disabled={isSubmitting || !selectedOption || !notificationPreference}
                                    aria-busy={isSubmitting}
                                >
                                    {isSubmitting ? translate("ConfirmAndSave.submitting") : translate("ConfirmAndSave.savePreferences")}
                                </button>
                            </form>

                            <hr />

                            <p className="nhsuk-hint" style={{ marginBottom: "1rem" }}>
                                If you have changed your mind and want to update your choice, click below to go back.
                            </p>
                            <button
                                className="nhsuk-button nhsuk-button--secondary"
                                type="button"
                                style={{ width: "100%", marginBottom: "1rem" }}
                                onClick={handleBack}
                                data-testid="back-btn"
                            >
                                <FontAwesomeIcon icon={faArrowLeftLong} /> Go Back
                            </button>
                            <p className="nhsuk-hint" style={{ marginBottom: "1.5rem" }}>
                                <strong>You can also change your mind at any time by returning to this site.</strong>
                            </p>
                        </div>
                    </div>

                    {apiError && (
                        <Alert variant="danger" onClose={() => setApiError("")} dismissible data-testid="error-alert">
                            {apiError}
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
                        }}
                        data-testid="help-guidance-section"
                    >
                        <h2 className="mb-3" style={{ color: "#005eb8" }} data-testid="help-guidance-heading">{translate("ConfirmAndSave.helpGuidanceTitle")}</h2>
                        <h3 data-testid="about-this-step-heading">{translate("ConfirmAndSave.aboutThisStepTitle")}</h3>
                        <p>
                            {translate("ConfirmAndSave.aboutThisStepDesc1")}
                        </p>
                        <p>
                            <strong>What to expect:</strong><br />
                            - If you chose to be notified, you will receive updates when your data is used by the London Data Service.<br />
                            - If you opted out of notifications, you will not receive updates, but your choice is still recorded and respected.
                        </p>
                        <p>
                            <strong>Your rights and privacy:</strong><br />
                            You are in control of your confidential patient information. The NHS takes your privacy seriously and your information will never be used for marketing or insurance purposes. You can change your data sharing choice at any time.
                        </p>
                        <p>
                            <strong>Need more information?</strong><br />
                            Visit the&nbsp;
                            <a
                                href="https://www.nhs.uk/your-nhs-data-matters/"
                                target="_blank"
                                rel="noopener noreferrer"
                                style={{ color: "#005eb8", textDecoration: "underline" }}
                            >
                                NHS Your Data Matters
                            </a>
                            &nbsp;page to learn more about how your data is used and your choices.
                        </p>
                        <p>
                            <strong>{translate("ConfirmAndSave.needHelpTitle")}</strong><br />

                            {translate("ConfirmAndSave.needHelpDesc")}

                            on &nbsp;
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
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default ConfirmationNhsLogin;