import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { decisionViewService } from "../../services/views/decisionViewService";
import { PatientDecision } from "../../models/patientDecisions/patientDecision";
import { isAxiosError } from "../../helpers/axiosErrorHelper";
import { useTranslation } from "react-i18next";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { mapValidationCodeToNumber } from "../../helpers/mapValidationCodeToNumber";
import { useNavigate } from "react-router-dom";
import { faArrowLeftLong } from "@fortawesome/free-solid-svg-icons/faArrowLeftLong";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface ConfirmationProps {
    selectedOption: "optout" | "optin" | null;
    createdPatient?: Patient | null;
    powerOfAttorney?: PowerOfAttourney | null;
}

export const Confirmation: React.FC<ConfirmationProps> = ({
    selectedOption,
    createdPatient,
    powerOfAttorney
}) => {

    const createDecisionMutation = decisionViewService.useCreatePatientDecision();
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { t: translate } = useTranslation();
    const { configuration } = useFrontendConfiguration();
    const navigate = useNavigate()

    const handleBack = () => {
        navigate("/optInOut", { state: { createdPatient, powerOfAttorney } });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!createdPatient?.nhsNumber || !selectedOption) {
            setError(translate("ConfirmAndSave.errorMissingNhsOrOption"));
            return;
        }

        setError(null);
        setIsSubmitting(true);

        const decision = new PatientDecision({
            id: crypto.randomUUID(),
            patientId: createdPatient?.id,
            patient: {
                nhsNumber: createdPatient?.nhsNumber || "",
                validationCode: createdPatient?.validationCode,
            },
            decisionChoice: selectedOption,
            decisionTypeId: configuration.decisionTypeId,
            responsiblePersonGivenName: powerOfAttorney?.firstName,
            responsiblePersonRelationship: powerOfAttorney?.relationship,
            responsiblePersonSurname: powerOfAttorney?.surname
        });

        createDecisionMutation.mutate(
            decision,
            {
                onSuccess: () => {
                    setIsSubmitting(false);
                    navigate("/thankyou")
                },
                onError: (error: unknown) => {
                    setIsSubmitting(false);
                    let message = translate("ConfirmAndSave.errorSaveFailed");
                    if (error instanceof Error && error.message) {
                        if (error.message === "Network Error") {
                            message = translate("ConfirmAndSave.errorSaveFailed");
                        } else {
                            message = error.message;
                        }
                    } else if (typeof error === "string") {
                        message = error;
                    } else if (isAxiosError(error)) {
                        const data = error.response?.data;
                        // eslint-disable-next-line @typescript-eslint/no-explicit-any
                        if (data && typeof data === "object" && "message" in data && typeof (data as any).message === "string") {
                            message = (data as { message: string }).message;
                        }
                    }
                    setError(message);
                }
            });

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
                                            {createdPatient?.nhsNumber || translate("ConfirmAndSave.nhsNumberNotProvided")}
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
                                <strong>You can change your mind at any time by returning to this site.</strong>
                            </p>
                        </div>
                    </div>


                    {error && (
                        <Alert variant="danger" onClose={() => setError("")} dismissible data-testid="error-alert">
                            {error}
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