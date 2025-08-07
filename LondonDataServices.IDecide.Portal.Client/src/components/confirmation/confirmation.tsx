import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../../hooks/useStep";
import { decisionViewService } from "../../services/views/decisionViewService";
import { Decision } from "../../models/decisions/decision";
import { isAxiosError } from "../../helpers/axiosErrorHelper";
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
            setError("NHS number and option are required.");
            return;
        }

        setError(null);

        const decision = new Decision({
            patientNhsNumber: nhsNumber,
            decisionChoice: selectedOption,
        });

        createDecisionMutation.mutate(decision, {
            onSuccess: () => {
                nextStep();
            },
            onError: (error: unknown) => {
                let message = "Sorry, we couldn't save your decision. Please try again.";
                if (error instanceof Error && error.message) {
                    message = error.message;
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
                    <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                        <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                        </div>

                        <div>
                            <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                Your Data Sharing Choice
                            </div>
                            <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>Decision:</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong>{selectedOption === "optin" ? "Opt-In" : selectedOption === "optout" ? "Opt-Out" : "Not selected"}</strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>NHS Number: &nbsp;</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong>{nhsNumber || "Not provided"}</strong>
                                    </dd>
                                </div>
                                <div>
                                    <dt style={{ display: "inline", fontWeight: 500 }}>Notification Method:</dt>
                                    <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                        <strong>
                                            {selectedMethods.length > 0
                                                ? selectedMethods.join(", ")
                                                : "None selected"}
                                        </strong>
                                    </dd>
                                </div>
                            </dl>
                            {powerOfAttourney && (
                                <>
                                    <hr />
                                    <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                        Power of Attorney Details
                                    </div>
                                    <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>Name of Requester:</dt>
                                            <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                                <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                            </dd>
                                        </div>
                                        <div>
                                            <dt style={{ display: "inline", fontWeight: 500 }}>Requesters Relationship:</dt>
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
                        <Alert variant="danger" onClose={() => setError(null)} dismissible>
                            {error}
                        </Alert>
                    )}

                    <form className="nhsuk-form-group" onSubmit={handleSubmit}>
                        <label className="nhsuk-label" style={{ marginBottom: "1rem" }}>
                            How would you like to be notified when your data has flowed into The London Data Service:
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
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="SMS">
                                    SMS
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
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="Email">
                                    Email
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
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="Post">
                                    Post
                                </label>
                            </div>
                        </div>

                        <button className="nhsuk-button" type="submit" style={{ width: "100%" }}>
                            Save Preferences
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
                    >
                        <h2 className="mb-3" style={{ color: "#005eb8" }}>Help & Guidance</h2>
                        <h3>About this step</h3>
                        <p>
                            You are about to save your opt-out preference. This means you are choosing how your data will be shared for secondary uses within The London Data Service.
                        </p>
                        <p>
                            Please select how you would like to be notified when your data has flowed into The London Data Service. You can choose to receive updates by SMS, email, or letter.
                        </p>
                        <ul>
                            <li><strong>SMS</strong> - a text message will be sent to your mobile phone</li>
                            <li><strong>Email</strong> - a message will be sent to your registered email address</li>
                            <li><strong>Letter</strong> - a letter will be sent to your home address (please allow up to 3 days for delivery)</li>
                        </ul>
                        <p>
                            You can return to this site at any time to change your notification preferences.
                        </p>
                        <h3>Need help?</h3>
                        <p>
                            If you have any questions or need assistance, please contact our helpdesk.
                        </p>
                    </div>
                </Col>
            </Row>
        </>
    );
};

export default Confirmation;