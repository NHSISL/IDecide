import React, { useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { Row, Col } from "react-bootstrap";

interface OptInOptOutProps {
    createdPatient: Patient | null;
}

export const OptInOptOut: React.FC<OptInOptOutProps> = ({ createdPatient }) => {
    const [selectedOption, setSelectedOption] = useState<"optout" | "optin" | "">("");
    const [error, setError] = useState("");
    const { nextStep } = useStep();

    if (!createdPatient) {
        return (
            <div className="nhsuk-error-message" role="alert">
                <strong>Error:</strong> Patient information is missing.
            </div>
        );
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedOption) {
            setError("Please select an option to continue.");
            return;
        }
        setError("");
        nextStep(selectedOption, createdPatient.nhsNumber);
    };

    return (
        <>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
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
                                    onChange={() => setSelectedOption("optout")}
                                    style={{ marginRight: "1rem", marginTop: "0.2rem" }}
                                    aria-describedby="optout-desc"
                                />
                                <div>
                                    <strong>Opt-Out</strong>
                                    <div id="optout-desc" style={{ marginTop: "0.5rem" }}>
                                        <div>
                                            I do not want my personal data to be used in the London Data Service for Research and Commissioning purposes.
                                        </div>
                                        <div style={{ marginTop: "0.5rem", color: "#505a5f" }}>
                                            I acknowledge that my data will still reside in the London Data Service for direct care purposes.
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
                                    onChange={() => setSelectedOption("optin")}
                                    style={{ marginRight: "1rem", marginTop: "0.2rem" }}
                                    aria-describedby="optin-desc"
                                />
                                <div>
                                    <strong>Opt-In</strong>
                                    <div id="optin-desc" style={{ marginTop: "0.5rem" }}>
                                        I do want my personal data to be used in the London Data Service for Direct Care, Research and Commissioning purposes.
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
                            Next
                        </button>
                    </form>
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
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>Help & Guidance</h2>
                        <h3>Whats does Opt-Out do?</h3>
                        <p>Choosing to opt out of sharing your data will stop your anonymised health information being used for:</p>
                        <ul>
                            <li>Supporting the provision of direct & proactive healthcare</li>
                            <li>Planning & Commissioning of Healthcare Services</li>
                            <li>Assessing & Improving Healthcare Services</li>
                            <li>Enabling Research</li>
                        </ul>
                        <p>
                            Choosing to opt-out of sharing your data for secondary purposes means your personal health information will still
                            be used to make sure you get the treatment and care you need. For example, your data may be shared if you
                            need to be referred to hospital or get a prescription.
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default OptInOptOut;