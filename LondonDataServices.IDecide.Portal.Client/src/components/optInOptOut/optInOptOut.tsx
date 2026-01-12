import React, { useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from 'react-i18next';

interface OptInOptOutProps {
    createdPatient: Patient | null;
}

export const OptInOptOut: React.FC<OptInOptOutProps> = ({ createdPatient }) => {
    const [selectedOption, setSelectedOption] = useState<"optout" | "optin" | "">("");
    const [error, setError] = useState("");
    const { nextStep, powerOfAttorney } = useStep();
    const { t: translate } = useTranslation();

    if (!createdPatient) {
        return (
            <div className="nhsuk-error-message" role="alert">
                <strong>Error:</strong> {translate("OptOut.errorNoPatient")}
            </div>
        );
    }

    const handleOptionChange = (option: "optout" | "optin") => {
        setSelectedOption(option);
        if (error) setError("");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedOption) {
            setError(translate("OptOut.errorSelectOption"));
            return;
        }
        setError("");
        nextStep(selectedOption, createdPatient.nhsNumber, createdPatient, powerOfAttorney ?? undefined);
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={12} lg={7}>
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

                    <h4 style={{ fontWeight: 700, fontSize: "1.5rem", margin: "0 0 1rem 0", color: "#212529" }}>
                        {translate("ConfirmDetails.confirmDetails", "Make your choice")}
                    </h4>

                    <form onSubmit={handleSubmit}>
                        {/* Opt-Out Card */}
                        <div
                            className="nhsuk-card"
                            style={{
                                border: selectedOption === "optout" ? "2px solid #005eb8" : "1px solid #d8dde0",
                                marginBottom: "2rem",
                                padding: "1.5rem",
                                borderRadius: "6px",
                                background: "#fff"
                            }}
                        >
                            <label style={{ display: "flex", alignItems: "flex-start", cursor: "pointer" }}>
                                <input
                                    type="radio"
                                    name="opt"
                                    value="optout"
                                    checked={selectedOption === "optout"}
                                    onChange={() => handleOptionChange("optout")}
                                    style={{ marginRight: "1rem", marginTop: "0.2rem" }}
                                    aria-describedby="optout-desc"
                                />
                                <div>
                                    <strong>{translate("OptOut.optOutLabel")}</strong>
                                    <div id="optout-desc" style={{ marginTop: "0.5rem" }}>
                                        <div>
                                            {translate("OptOut.optOutDesc1")}
                                        </div>
                                        <div style={{ marginTop: "0.5rem", color: "#505a5f" }}>
                                            {translate("OptOut.optOutDesc2")}
                                        </div>
                                    </div>
                                </div>
                            </label>
                        </div>

                        {/* Opt-In Card */}
                        <div
                            className="nhsuk-card"
                            style={{
                                border: selectedOption === "optin" ? "2px solid #005eb8" : "1px solid #d8dde0",
                                padding: "1.5rem",
                                borderRadius: "6px",
                                background: "#fff"
                            }}
                        >
                            <label style={{ display: "flex", alignItems: "flex-start", cursor: "pointer" }}>
                                <input
                                    type="radio"
                                    name="opt"
                                    value="optin"
                                    checked={selectedOption === "optin"}
                                    onChange={() => handleOptionChange("optin")}
                                    style={{ marginRight: "1rem", marginTop: "0.2rem" }}
                                    aria-describedby="optin-desc"
                                />
                                <div>
                                    <strong>{translate("OptOut.optInLabel")}</strong>
                                    <div id="optin-desc" style={{ marginTop: "0.5rem" }}>
                                        {translate("OptOut.optInDesc1")}
                                    </div>
                                    <div id="optin-desc" style={{ marginTop: "0.5rem", color: "#505a5f" }}>
                                        {translate("OptOut.optInDesc2")}
                                    </div>
                                </div>
                            </label>
                        </div>

                        {error && (
                            <div className="nhsuk-error-message" style={{ marginBottom: "1rem" }} role="alert">
                                <strong>Error:</strong> {error}
                            </div>
                        )}

                        <button
                            type="submit"
                            className="nhsuk-button"
                            style={{ width: "100%", marginTop: "0.2rem" }}
                            disabled={!selectedOption}
                        >
                            {translate("OptOut.nextButton")}
                        </button>
                    </form>
                </Col>
                <Col xs={12} md={12} lg={5} className="custom-col-spacing">
                    <div
                        className="p-4 mb-4"
                        style={{
                            background: "#f4f8fb",
                            border: "1px solid #d1e3f0",
                            borderRadius: "8px",
                            boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                        }}
                    >
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>
                            {translate("OptOut.helpGuidanceTitle")}
                        </h2>
                        <h3>{translate("OptOut.helpOptOutTitle")}</h3>
                        <p>
                            Your confidential patient information can be used for improving health, care and services, including:
                        </p>
                        <ul>
                            <li>Planning to improve health and care services</li>
                            <li>Research to find a cure for serious illnesses</li>
                        </ul>
                        <p>
                            Your decision will not affect your individual care and you can change your mind at any time.
                        </p>
                        <p>
                            <strong>Why is this important?</strong>
                            The NHS uses information from millions of patients to plan and improve services, and to support vital research. This helps ensure everyone receives the best possible care.
                        </p>
                        <p>
                            <strong>Your rights:</strong>
                            You are in control of your confidential patient information. You can choose whether your data is used for research and planning, and you can change your choice at any time without it affecting your care.
                        </p>
                        <p>
                            <strong>Need more information?</strong>
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
                            <strong>Security reminder:</strong>
                            The NHS takes your privacy seriously. Your information is protected and will never be used for marketing or insurance purposes.
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default OptInOptOut;