import React from "react";
import { useNavigate } from "react-router-dom";

const PositiveConfirmation = () => {
    const navigate = useNavigate();

    const handleSubmit = (method: "email" | "sms" | "letter") => {
        console.log(method);
        // Navigate to confirmCode route
        navigate("/confirmCode");
    };

    return (
        <div className="mt-4">
            <h2>Confirmation required</h2>
            <p>Please confirm these details are correct before continuing:</p>
            <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Name</dt>
                    <dd className="nhsuk-summary-list__value">foo</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Email</dt>
                    <dd className="nhsuk-summary-list__value">pow</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                    <dd className="nhsuk-summary-list__value">muuhahahaha</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Address</dt>
                    <dd className="nhsuk-summary-list__value">junk</dd>
                </div>
            </dl>

            <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                We need to send a code to you, how would you like to receive it:
            </p>
            <div
                style={{
                    display: "flex",
                    gap: "1rem",
                    marginBottom: "2rem",
                    flexWrap: "wrap",
                }}
            >
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={() => handleSubmit("email")}
                >
                    Email
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={() => handleSubmit("sms")}
                >
                    SMS
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={() => handleSubmit("letter")}
                >
                    Letter
                </button>
            </div>
        </div>
    );
};

export default PositiveConfirmation;
