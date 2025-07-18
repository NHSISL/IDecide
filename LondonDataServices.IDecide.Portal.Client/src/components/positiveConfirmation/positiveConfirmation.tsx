import React from "react";

interface PositiveConfirmationProps {
    onBack: () => void;
    goToConfirmCode: () => void;
}

const PositiveConfirmation: React.FC<PositiveConfirmationProps> = ({ onBack, goToConfirmCode }) => {
    const details = {
        name: "Mr D**** H****",
        email: "d****hay**@googlemail.com",
        mobile: "07******084",
        address: "9 T** W********* Cr2 ***"
    };

    return (
        <div className="mt-4">
            <h2>Confirmation required</h2>
            <p>Please confirm these details are correct before continuing:</p>
            <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Name</dt>
                    <dd className="nhsuk-summary-list__value">{details.name}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Email</dt>
                    <dd className="nhsuk-summary-list__value">{details.email}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                    <dd className="nhsuk-summary-list__value">{details.mobile}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Address</dt>
                    <dd className="nhsuk-summary-list__value">{details.address}</dd>
                </div>
            </dl>

            <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                We need to send a code to you, how would you like to receive it:
            </p>
            <div style={{
                display: "flex",
                gap: "1rem",
                marginBottom: "2rem",
                flexWrap: "wrap"
            }}>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={goToConfirmCode}
                >
                    Email
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={goToConfirmCode}
                >
                    SMS
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={goToConfirmCode}
                >
                    Letter
                </button>
            </div>
        </div>
    );
};

export default PositiveConfirmation;