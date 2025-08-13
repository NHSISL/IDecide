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
    const { nextStep, powerOfAttourney } = useStep();
    const { t } = useTranslation();

    if (!createdPatient) {
        return (
            <div className="nhsuk-error-message" role="alert">
                <strong>Error:</strong> {t("OptOut.errorNoPatient")}
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
            setError(t("OptOut.errorSelectOption"));
            return;
        }
        setError("");
        nextStep(selectedOption, createdPatient.nhsNumber);
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={12} lg={7}>
                    {powerOfAttourney && (
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    {t("OptOut.powerOfAttorneyDetails")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{t("OptOut.powerOfAttorneyName")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{t("OptOut.powerOfAttorneyRelationship")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}

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
                                    <strong>{t("OptOut.optOutLabel")}</strong>
                                    <div id="optout-desc" style={{ marginTop: "0.5rem" }}>
                                        <div>
                                            {t("OptOut.optOutDesc1")}
                                        </div>
                                        <div style={{ marginTop: "0.5rem", color: "#505a5f" }}>
                                            {t("OptOut.optOutDesc2")}
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
                                    <strong>{t("OptOut.optInLabel")}</strong>
                                    <div id="optin-desc" style={{ marginTop: "0.5rem" }}>
                                        {t("OptOut.optInDesc")}
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
                        >
                            {t("OptOut.nextButton")}
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
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>{t("OptOut.helpGuidanceTitle")}</h2>
                        <h3>{t("OptOut.helpOptOutTitle")}</h3>
                        <p>{t("OptOut.helpOptOutDesc1")}</p>
                        <ul>
                            <li>{t("OptOut.helpOptOutList1")}</li>
                            <li>{t("OptOut.helpOptOutList2")}</li>
                            <li>{t("OptOut.helpOptOutList3")}</li>
                            <li>{t("OptOut.helpOptOutList4")}</li>
                        </ul>
                        <p>
                            {t("OptOut.helpOptOutDesc2")}
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default OptInOptOut;