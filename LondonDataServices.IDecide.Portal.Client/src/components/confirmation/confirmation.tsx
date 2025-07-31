import React, { useState } from "react";
import { Alert, Col, Row } from "react-bootstrap";
import { useStep } from "../context/stepContext";
import { decisionViewService } from "../../services/views/decisionViewService";
import { Decision } from "../../models/decisions/decision";
import { isAxiosError } from "../../helpers/axiosErrorHelper";
interface ConfirmationProps {
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
}

export const Confirmation: React.FC<ConfirmationProps> = ({ selectedOption, nhsNumber }) => {
    const [prefs, setPrefs] = useState({
        sms: false,
        email: false,
        post: false,
    });

    const { nextStep } = useStep();
    const createDecisionMutation = decisionViewService.useCreateDecision();
    const [error, setError] = useState<string | null>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name } = e.target;
        setPrefs({
            sms: name === "sms",
            email: name === "email",
            post: name === "post",
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
            id: "", // Let backend generate or use uuid if needed
            patientNhsNumber: nhsNumber,
            decisionChoice: selectedOption,
        });

        createDecisionMutation.mutate(decision, {
            onSuccess: (createdDecision) => {
                alert(`Your choice "${createdDecision.decisionChoice}" has been saved.`);
                nextStep();
            },
            onError: (error: unknown) => {
                let message = "Sorry, we couldn't save your decision. Please try again.";
                if (error instanceof Error && error.message) {
                    message = error.message;
                } else if (typeof error === "string") {
                    message = error;
                } else if (isAxiosError(error)) {
                    // Type guard for AxiosError
                    const data = error.response?.data;
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
                <Col xs={12} md={7} lg={7}>
                    <Alert variant="success">
                        <strong>Selected Option: &nbsp;</strong>
                        {selectedOption === "optin" ? "Opt-In" : selectedOption === "optout" ? "Opt-Out" : "Not selected"}
                        <br />
                        <strong>NHS Number: &nbsp;</strong>
                        {nhsNumber || "Not provided"}
                    </Alert>

                    {error && (
                        <Alert variant="danger" onClose={() => setError(null)} dismissible>
                            {error}
                        </Alert>
                    )}

                    <form className="nhsuk-form-group" onSubmit={handleSubmit}>
                        <p style={{ fontWeight: 500, marginBottom: "1.5rem" }}>
                            We will notify you when this has been enacted.
                        </p>
                        <label className="nhsuk-label" style={{ marginBottom: "1rem" }}>
                            How would you like to receive it:
                        </label>
                        <div className="nhsuk-checkboxes nhsuk-checkboxes--vertical" style={{ marginBottom: "1.5rem" }}>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="sms"
                                    name="sms"
                                    type="checkbox"
                                    checked={prefs.sms}
                                    onChange={handleChange}
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="sms">
                                    SMS
                                </label>
                            </div>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="email"
                                    name="email"
                                    type="checkbox"
                                    checked={prefs.email}
                                    onChange={handleChange}
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="email">
                                    EMAIL
                                </label>
                            </div>
                            <div className="nhsuk-checkboxes__item">
                                <input
                                    className="nhsuk-checkboxes__input"
                                    id="post"
                                    name="post"
                                    type="checkbox"
                                    checked={prefs.post}
                                    onChange={handleChange}
                                />
                                <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor="post">
                                    POST
                                </label>
                            </div>
                        </div>

                        <button className="nhsuk-button" type="submit" style={{ width: "100%" }}>
                            Save Preferences
                        </button>

                        <div style={{ color: "#505a5f" }}>
                            <strong>You can come back to this site and change your preference at any time.</strong>
                        </div>
                    </form>
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                </Col>
            </Row>
        </>
    );
};

export default Confirmation;