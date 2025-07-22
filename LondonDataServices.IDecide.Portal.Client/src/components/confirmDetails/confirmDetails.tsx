import React from "react";
import { useStep } from "../context/stepContext";

interface ConfirmDetailsProps {
    goToConfirmCode: () => void;
}

export const ConfirmDetails: React.FC<ConfirmDetailsProps> = ({ goToConfirmCode }) => {
    const { setCurrentStepIndex, nextStep, createdPatient } = useStep();

    const handleNoClick = () => {
        setCurrentStepIndex(0); 
    };

    const handleYesClick = () => {
        nextStep(createdPatient); 
    };

    if (!createdPatient) {
        return <div>No patient details available.</div>;
    }

    return (
        <div className="mt-4">
            <h4>Is this you?</h4>
            <form className="nhsuk-form-group">
                <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Name</dt>
                        <dd className="nhsuk-summary-list__value">{createdPatient.firstName +','+ createdPatient.surname}</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Email</dt>
                        <dd className="nhsuk-summary-list__value">{createdPatient.emailAddress}</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                        <dd className="nhsuk-summary-list__value">07******084</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Address</dt>
                        <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                    </div>
                </dl>

                <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem" }}>
                    <button
                        type="button"
                        className="nhsuk-button"
                        style={{ flex: 1 }}
                        onClick={handleYesClick}
                    >
                        Yes
                    </button>
                    <button
                        type="button"
                        className="nhsuk-button nhsuk-button--secondary"
                        style={{ flex: 1 }}
                        onClick={handleNoClick}
                    >
                        No
                    </button>
                </div>

                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ width: "100%", marginBottom: 0, borderBottom: "none" }}
                    onClick={goToConfirmCode}
                >
                    We have already sent you a code. Would you like to use that code to continue?
                </button>
            </form>
        </div>
    );
};

export default ConfirmDetails;