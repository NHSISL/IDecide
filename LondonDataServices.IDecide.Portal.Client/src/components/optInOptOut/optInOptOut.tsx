import React, { useState } from "react";
import { useStep } from "../context/stepContext";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { Patient } from "../../models/patients/patient";

interface OptInOptOutProps {
    createdPatient: Patient;
}

export const OptInOptOut: React.FC<OptInOptOutProps> = ({ createdPatient }) => {
    const [selectedOption, setSelectedOption] = useState<"optout" | "optin" | "">("");
    const [error, setError] = useState("");
    const [showInfo, setShowInfo] = useState(false);
    const { nextStep } = useStep();

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedOption) {
            setError("Please select an option to continue.");
            return;
        }
        setError("");

        console.log(createdPatient.nhsNumber)
        console.log(selectedOption)
        nextStep(selectedOption, createdPatient.nhsNumber);
    };

    React.useEffect(() => {
        if (!showInfo) return;
        const handler = () => setShowInfo(false);
        window.addEventListener("click", handler);
        return () => window.removeEventListener("click", handler);
    }, [showInfo]);

    return (
        <>
            <div style={{ position: "relative", marginBottom: "1.5rem" }}>
                <button
                    type="button"
                    style={{
                        display: "inline-flex",
                        alignItems: "center",
                        background: "none",
                        border: "none",
                        padding: 0,
                        cursor: "pointer",
                        outline: "none"
                    }}
                    tabIndex={0}
                    aria-label="What does Opt-out do? (show more info)"
                    onClick={e => {
                        e.stopPropagation();
                        setShowInfo(v => !v);
                    }}
                    onKeyDown={e => {
                        if (e.key === "Enter" || e.key === " ") {
                            e.preventDefault();
                            setShowInfo(v => !v);
                        }
                    }}
                >
                    <FontAwesomeIcon icon={faCircleInfo} style={{ color: "#005eb8" }} />
                    <span style={{ marginLeft: 8 }}>What does Opt-out do?</span>
                </button>
               
                {showInfo && (
                    <div
                        style={{
                            position: "absolute",
                            zIndex: 10,
                            top: "2.5rem",
                            left: 0,
                            maxWidth: "340px",
                            background: "#fff",
                            border: "1px solid #d8dde0",
                            borderRadius: "6px",
                            boxShadow: "0 2px 8px rgba(0,0,0,0.12)",
                            padding: "1rem",
                            fontSize: "1rem"
                        }}
                        onClick={e => e.stopPropagation()}
                        role="dialog"
                        aria-modal="true"
                    >
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
                )}
            </div>
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
                    Submit
                </button>
            </form>
        </>
    );
};

export default OptInOptOut;