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
    const [prefs, setPrefs] = useState({
        SMS: false,
        Email: false,
        Post: false,
    });

    const createDecisionMutation = decisionViewService.useCreatePatientDecision();
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { t: translate } = useTranslation();
    const { configuration } = useFrontendConfiguration();
    const navigate = useNavigate()

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
                notificationPreference: mapValidationCodeToNumber(methodForHelper)?.toString() ?? undefined
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
                                        <strong data-testid="nhs-number-value">{createdPatient?.nhsNumber || translate("ConfirmAndSave.nhsNumberNotProvided")}</strong>
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
                            
                        </div>

                    </Alert>

                    {powerOfAttorney && (
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    {translate("OptOut.powerOfAttorneyDetails")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("OptOut.powerOfAttorneyName")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttorney.firstName} {powerOfAttorney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("OptOut.powerOfAttorneyRelationship")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttorney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}



                    {error && (
                        <Alert variant="danger" onClose={() => setError(null)} dismissible data-testid="error-alert">
                            {error}
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
                            disabled={isSubmitting}
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