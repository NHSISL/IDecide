import React from "react";
import { useNavigate } from "react-router-dom";

export const ConfirmDetails = () => {
    const navigate = useNavigate();

    const handleSubmit = (choice: string) => {
        console.log("User clicked:", choice);
        switch (choice) {
            case "yes":
                navigate("/sendCode");
                break;
            case "no":
                navigate("/nhsNumberSearch");
                break;
            case "use-code":
                navigate("/confirmCode");
                break;
            default:
                break;
        }
    };

    return (
        <div className="mt-4">
            <h4>Is this you?</h4>
            <form className="nhsuk-form-group">
                <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Name</dt>
                        <dd className="nhsuk-summary-list__value">boo</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Email</dt>
                        <dd className="nhsuk-summary-list__value">Blah</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                        <dd className="nhsuk-summary-list__value">07******084</dd>
                    </div>
                    <div className="nhsuk-summary-list__row">
                        <dt className="nhsuk-summary-list__key">Address</dt>
                        <dd className="nhsuk-summary-list__value">foo</dd>
                    </div>
                </dl>

                <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem" }}>
                    <button
                        type="button"
                        className="nhsuk-button"
                        style={{ flex: 1 }}
                        onClick={() => handleSubmit("yes")}
                    >
                        Yes
                    </button>
                    <button
                        type="button"
                        className="nhsuk-button nhsuk-button--secondary"
                        style={{ flex: 1 }}
                        onClick={() => handleSubmit("no")}
                    >
                        No
                    </button>
                </div>

                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ width: "100%", marginBottom: 0, borderBottom: "none" }}
                    onClick={() => handleSubmit("use-code")}
                >
                    We have already sent you a code. Would you like to use that code to continue?
                </button>
            </form>
        </div>
    );
};

export default ConfirmDetails;
