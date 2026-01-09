import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../../hooks/useStep";
import { decisionViewService } from "../../services/views/decisionViewService";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { useTranslation } from "react-i18next";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { Patient } from "../../models/patients/patient";
import { PowerOfAttorney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";
import { faArrowLeftLong } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

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

    const { nextStep, previousStep } = useStep();
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

    const handleBack = () => {
        previousStep();
    };

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
                validationCode: createdPatient?.validationCode
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


                    <div className="nhsuk-card nhsuk-card--summary">
                        <div className="nhsuk-card__content">
                            <h3 className="nhsuk-card__heading">Your Data Sharing Choice</h3>

                            <dl className="nhsuk-summary-list">
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
                                    <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>NHS Number</dt>
                                    <dd className="nhsuk-summary-list__value">
                                        <strong data-testid="nhs-number-value">
                                            {nhsNumber || translate("ConfirmAndSave.nhsNumberNotProvided")}
                                        </strong>
                                    </dd>
                                </div>
                            </dl>

                            {powerOfAttorney && (
                                <>
                                    <hr />
                                    <h3 className="nhsuk-card__heading nhsuk-u-margin-top-4">
                                        {translate("ConfirmAndSave.powerOfAttorneyDetails")}
                                    </h3>
                                    <dl className="nhsuk-summary-list">
                                        <div className="nhsuk-summary-list__row">
                                            <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>
                                                {translate("ConfirmAndSave.powerOfAttorneyName")}
                                            </dt>
                                            <dd className="nhsuk-summary-list__value">
                                                <strong>{powerOfAttorney.firstName} {powerOfAttorney.surname}</strong>
                                            </dd>
                                        </div>

                                        <div className="nhsuk-summary-list__row">
                                            <dt className="nhsuk-summary-list__key" style={{ fontWeight: "lighter" }}>
                                                {translate("ConfirmAndSave.powerOfAttorneyRelationship")}
                                            </dt>
                                            <dd className="nhsuk-summary-list__value">
                                                <strong>{powerOfAttorney.relationship}</strong>
                                            </dd>
                                        </div>
                                    </dl>
                                </>
                            )}

                            <hr />
                            <form className="nhsuk-form-group" onSubmit={handleSubmit} data-testid="confirmation-form" >
                                <button
                                    className="nhsuk-button"
                                    type="submit"
                                    style={{ width: "100%", marginBottom: "5px" }}
                                    data-testid="save-preferences-btn"
                                    disabled={isSubmitting || !selectedOption}
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