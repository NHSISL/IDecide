import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../../hooks/useStep";
import { decisionViewService } from "../../services/views/decisionViewService";
import { Decision } from "../../models/decisions/decision";
import { isAxiosError } from "../../helpers/axiosErrorHelper";
import { useTranslation } from "react-i18next";

interface ConfirmationProps {
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
}

export const Confirmation: React.FC<ConfirmationProps> = ({ selectedOption, nhsNumber }) => {
    const [prefs, setPrefs] = useState({
        SMS: false,
        Email: false,
        Post: false,
    });

    const { nextStep, powerOfAttourney } = useStep();
    const createDecisionMutation = decisionViewService.useCreateDecision();
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { t } = useTranslation();

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

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (!nhsNumber || !selectedOption) {
            setError(t("ConfirmAndSave.errorMissingNhsOrOption"));
            return;
        }

        setError(null);
        setIsSubmitting(true);

        const decision = new Decision({
            patientNhsNumber: nhsNumber,
            decisionChoice: selectedOption,
        });

        createDecisionMutation.mutate(decision, {
            onSuccess: () => {
                setIsSubmitting(false);
                nextStep();
            },
            onError: (error: unknown) => {
                setIsSubmitting(false);
                let message = t("ConfirmAndSave.errorSaveFailed");
                if (error instanceof Error && error.message) {
                    if (error.message === "Network Error") {
                        message = t("ConfirmAndSave.errorSaveFailed");
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

    const selectedMethods = Object.entries(prefs)
        .filter(([, value]) => value)
        .map(([key]) => key);

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
                                {t("ConfirmAndSave.yourDataSharingChoice")}
                            </div>
                            <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmAndSave.decisionLabel")}</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="decision-value">
                                            {selectedOption === "optin"
                                                ? t("ConfirmAndSave.decisionOptIn")
                                                : selectedOption === "optout"
                                                    ? t("ConfirmAndSave.decisionOptOut")
                                                    : t("ConfirmAndSave.decisionNotSelected")}
                                        </strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmAndSave.nhsNumberLabel")}&nbsp;</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="nhs-number-value">{nhsNumber || t("ConfirmAndSave.nhsNumberNotProvided")}</strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmAndSave.notificationMethodLabel")}</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong data-testid="notification-method-value">
                                            {selectedMethods.length > 0
                                                ? selectedMethods.join(", ")
                                                : t("ConfirmAndSave.notificationNoneSelected")}
                                        </strong>
                                    </dd>
                                </div>
                            </dl>
                            {powerOfAttourney && (
                                <>
                                    <hr />
                                    <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                        {t("ConfirmAndSave.powerOfAttorneyDetails")}
                                    </div>
                                    <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmAndSave.powerOfAttorneyName")}</dt>
                                            <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                                <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                            </dd>
                                        </div>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>{t("ConfirmAndSave.powerOfAttorneyRelationship")}</dt>
                                            <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                                <strong>{powerOfAttourney.relationship}</strong>
                                            </dd>
                                        </div>
                                    </dl>
                                </>
                            )}
                        </div>

                    </Alert>

                    {error && (
                        <Alert variant="danger" onClose={() => setError(null)} dismissible data-testid="error-alert">
                            {error}
                        </Alert>
                    )}

                    <form className="nhsuk-form-group" onSubmit={handleSubmit} data-testid="confirmation-form">
                        <label className="nhsuk-label" style={{ marginBottom: "1rem" }}>
                            {t("ConfirmAndSave.howToBeNotifiedLabel")}
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
                                    {t("ConfirmAndSave.sms")}
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
                                    {t("ConfirmAndSave.email")}
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
                                    {t("ConfirmAndSave.post")}
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
                            {isSubmitting ? t("ConfirmAndSave.submitting") : t("ConfirmAndSave.savePreferences")}
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
                        <h2 className="mb-3" style={{ color: "#005eb8" }} data-testid="help-guidance-heading">{t("ConfirmAndSave.helpGuidanceTitle")}</h2>
                        <h3 data-testid="about-this-step-heading">{t("ConfirmAndSave.aboutThisStepTitle")}</h3>
                        <p>
                            {t("ConfirmAndSave.aboutThisStepDesc1")}
                        </p>
                        <p>
                            {t("ConfirmAndSave.aboutThisStepDesc2")}
                        </p>
                        <ul>
                            <li><strong>{t("ConfirmAndSave.helpSms")}</strong></li>
                            <li><strong>{t("ConfirmAndSave.helpEmail")}</strong></li>
                            <li><strong>{t("ConfirmAndSave.helpLetter")}</strong></li>
                        </ul>
                        <p>
                            {t("ConfirmAndSave.helpChangePrefs")}
                        </p>
                        <h3 data-testid="need-help-heading">{t("ConfirmAndSave.needHelpTitle")}</h3>
                        <p>
                            {t("ConfirmAndSave.needHelpDesc")}
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default Confirmation;