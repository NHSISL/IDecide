import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../../hooks/useStep";
import { decisionViewService } from "../../services/views/decisionViewService";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { useTranslation } from "react-i18next";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { Patient } from "../../models/patients/patient";
import { PowerOfAttorney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { mapValidationCodeToNumber } from "../../helpers/mapValidationCodeToNumber";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";

interface ConfirmationProps {
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
    createdPatient?: Patient | null;
    powerOfAttorney?: PowerOfAttorney | null;
}

export const Confirmation: React.FC<ConfirmationProps> = ({
    selectedOption,
    nhsNumber,
    createdPatient,
    powerOfAttorney
}) => {
    const [prefs, setPrefs] = useState({
        SMS: false,
        Email: false,
        Post: false,
    });

    const { nextStep } = useStep();
    const createDecisionMutation = decisionViewService.useCreatePatientDecision();
    const [apiError, setApiError] = useState<string | JSX.Element>("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { t: translate } = useTranslation();
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";

    const handleApiError = useApiErrorHandlerChecks({
        setApiError,
        configuration
    });

    // Only one method can be selected at a time
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name } = e.target;
        setPrefs({
            SMS: false,
            Email: false,
            Post: false,
            [name]: true,
        });
    };

    const selectedMethods = Object.entries(prefs)
        .filter(([, value]) => value)
        .map(([key]) => key);

    const selectedMethod = selectedMethods[0];

    const methodForHelper =
        selectedMethod === "SMS" ? "Sms" :
            selectedMethod === "Email" ? "Email" :
                selectedMethod === "Post" ? "letter" :
                    undefined;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!nhsNumber || !selectedOption) {
            setApiError(translate("ConfirmAndSave.errorMissingNhsOrOption"));
            return;
        }

        setApiError("");
        setIsSubmitting(true);

        const decision = new PatientDecision({
            id: crypto.randomUUID(),
            patientId: createdPatient?.id,
            patient: {
                nhsNumber: nhsNumber || "",
                validationCode: createdPatient?.validationCode,
                notificationPreference: mapValidationCodeToNumber(methodForHelper) ?? undefined
            },
            decisionChoice: selectedOption,
            decisionTypeId: configuration.decisionTypeId,
            responsiblePersonGivenName: powerOfAttorney?.firstName,
            responsiblePersonRelationship: powerOfAttorney?.relationship,
            responsiblePersonSurname: powerOfAttorney?.surname
        });

        try {
            const token = await grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: RECAPTCHA_ACTION_SUBMIT });

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

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
                    <Alert
                        variant="info"
                        className="d-flex align-items-center"
                        style={{ marginBottom: "0.75rem", padding: "0.75rem" }}
                        data-testid="confirmation-root"
                    >
                        <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                        </div>

                        <div>
                            <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                {translate("ConfirmAndSave.yourDataSharingChoice")}
                            </div>
                            <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmAndSave.decisionLabel")}</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="decision-value">
                                            {selectedOption === "optin"
                                                ? translate("ConfirmAndSave.decisionOptIn")
                                                : selectedOption === "optout"
                                                    ? translate("ConfirmAndSave.decisionOptOut")
                                                    : translate("ConfirmAndSave.decisionNotSelected")}
                                        </strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmAndSave.nhsNumberLabel")}&nbsp;</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="nhs-number-value">{nhsNumber || translate("ConfirmAndSave.nhsNumberNotProvided")}</strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmAndSave.notificationMethodLabel")}</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="notification-method-value">
                                            {selectedMethod
                                                ? selectedMethod
                                                : translate("ConfirmAndSave.notificationNoneSelected")}
                                        </strong>
                                    </dd>
                                </div>
                            </dl>
                            {powerOfAttorney && (
                                <>
                                    <hr />
                                    <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                        {translate("ConfirmAndSave.powerOfAttorneyDetails")}
                                    </div>
                                    <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmAndSave.powerOfAttorneyName")}</dt>
                                            <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                                <strong>{powerOfAttorney.firstName} {powerOfAttorney.surname}</strong>
                                            </dd>
                                        </div>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmAndSave.powerOfAttorneyRelationship")}</dt>
                                            <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                                <strong>{powerOfAttorney.relationship}</strong>
                                            </dd>
                                        </div>
                                    </dl>
                                </>
                            )}
                        </div>
                        
                    </Alert>

                    {apiError && (
                        <Alert variant="danger" onClose={() => setApiError("")} dismissible data-testid="error-alert">
                            {apiError}
                        </Alert>
                    )}

                    <form className="nhsuk-form-group" onSubmit={handleSubmit} data-testid="confirmation-form">
                        <label className="nhsuk-label" style={{ marginBottom: "1rem" }}>
                            {translate("ConfirmAndSave.howToBeNotifiedLabel")}
                        </label>
                        <div className="nhsuk-checkboxes nhsuk-checkboxes--vertical" style={{ marginBottom: "1.5rem" }}>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="sms"
                                    name="SMS"
                                    type="checkbox"
                                    checked={prefs.SMS}
                                    onChange={handleChange}
                                    data-testid="checkbox-sms"
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="SMS" data-testid="label-sms">
                                    {translate("ConfirmAndSave.sms")}
                                </label>
                            </div>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="email"
                                    name="Email"
                                    type="checkbox"
                                    checked={prefs.Email}
                                    onChange={handleChange}
                                    data-testid="checkbox-email"
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="Email" data-testid="label-email">
                                    {translate("ConfirmAndSave.email")}
                                </label>
                            </div>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="post"
                                    name="Post"
                                    type="checkbox"
                                    checked={prefs.Post}
                                    onChange={handleChange}
                                    data-testid="checkbox-post"
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="Post" data-testid="label-post">
                                    {translate("ConfirmAndSave.post")}
                                </label>
                            </div>
                        </div>

                        <button
                            className="nhsuk-button"
                            type="submit"
                            style={{ width: "100%" }}
                            data-testid="save-preferences-btn"
                            disabled={isSubmitting || !selectedOption || !selectedMethod}
                            aria-busy={isSubmitting}
                        >
                            {isSubmitting ? translate("ConfirmAndSave.submitting") : translate("ConfirmAndSave.savePreferences")}
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
                        data-testid="help-guidance-section"
                    >
                        <h2 className="mb-3" style={{ color: "#005eb8" }} data-testid="help-guidance-heading">{translate("ConfirmAndSave.helpGuidanceTitle")}</h2>
                        <h3 data-testid="about-this-step-heading">{translate("ConfirmAndSave.aboutThisStepTitle")}</h3>
                        <p>
                            {translate("ConfirmAndSave.aboutThisStepDesc1")}
                        </p>
                        <p>
                            {translate("ConfirmAndSave.aboutThisStepDesc2")}
                        </p>
                        <ul>
                            <li><strong>{translate("ConfirmAndSave.helpSms")}</strong></li>
                            <li><strong>{translate("ConfirmAndSave.helpEmail")}</strong></li>
                            <li><strong>{translate("ConfirmAndSave.helpLetter")}</strong></li>
                        </ul>
                        <p>
                            {translate("ConfirmAndSave.helpChangePrefs")}
                        </p>
                        <h3 data-testid="need-help-heading">{translate("ConfirmAndSave.needHelpTitle")}</h3>
                        <p>
                            {translate("ConfirmAndSave.needHelpDesc")}
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default Confirmation;