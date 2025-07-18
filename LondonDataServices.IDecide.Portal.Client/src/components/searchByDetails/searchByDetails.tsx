import React, { useState } from "react";

interface SearchByDetailsProps {
    onBack: () => void;
    nextStep: () => void;
}

const SearchByDetails: React.FC<SearchByDetailsProps> = ({ onBack, nextStep }) => {
    const [surname, setSurname] = useState("");
    const [postcode, setPostcode] = useState("");
    const [dobDay, setDobDay] = useState("");
    const [dobMonth, setDobMonth] = useState("");
    const [dobYear, setDobYear] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        // Simple validation
        if (!surname || !postcode || !dobDay || !dobMonth || !dobYear) {
            setError("All fields are required.");
            return;
        }
        setError("");
        nextStep();
    };

    return (
        <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit} >
            {/* NHS Back Link */}
            <button
                type="button"
                className="nhsuk-back-link"
                onClick={onBack}
                style={{
                    display: "flex",
                    alignItems: "center",
                    background: "none",
                    border: "none",
                    color: "#005eb8",
                    cursor: "pointer",
                    marginBottom: "1.5rem",
                    fontSize: "1rem",
                    padding: 0
                }}
                aria-label="Back"
            >
                <svg
                    className="nhsuk-icon nhsuk-icon__chevron-left"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 24 24"
                    width="24"
                    height="24"
                    aria-hidden="true"
                    focusable="false"
                    style={{ marginRight: "0.5rem" }}
                >
                    <path fill="currentColor" d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" />
                </svg>
                Back
            </button>

            <label className="nhsuk-label" htmlFor="surname">
                Surname
            </label>
            <input
                className="nhsuk-input"
                id="surname"
                name="surname"
                type="text"
                autoComplete="family-name"
                value={surname}
                onChange={e => setSurname(e.target.value)}
                style={{ marginBottom: "1rem" }}
            />

            <label className="nhsuk-label" htmlFor="postcode">
                Postcode
            </label>
            <input
                className="nhsuk-input"
                id="postcode"
                name="postcode"
                type="text"
                autoComplete="postal-code"
                value={postcode}
                onChange={e => setPostcode(e.target.value)}
                style={{ marginBottom: "1rem" }}
            />

            <fieldset className="nhsuk-fieldset" style={{ marginBottom: "1rem" }}>
                <legend className="nhsuk-fieldset__legend nhsuk-label">
                    Date of birth
                </legend>
                <div className="nhsuk-date-input" id="dob">
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-day">
                            Day
                        </label>
                        <input
                            className="nhsuk-input nhsuk-date-input__input"
                            id="dob-day"
                            name="dob-day"
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            maxLength={2}
                            value={dobDay}
                            onChange={e => setDobDay(e.target.value.replace(/\D/g, ""))}
                            style={{ width: "3em" }}
                            autoComplete="bday-day"
                        />
                    </div>
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-month">
                            Month
                        </label>
                        <input
                            className="nhsuk-input nhsuk-date-input__input"
                            id="dob-month"
                            name="dob-month"
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            maxLength={2}
                            value={dobMonth}
                            onChange={e => setDobMonth(e.target.value.replace(/\D/g, ""))}
                            style={{ width: "3em" }}
                            autoComplete="bday-month"
                        />
                    </div>
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-year">
                            Year
                        </label>
                        <input
                            className="nhsuk-input nhsuk-date-input__input"
                            id="dob-year"
                            name="dob-year"
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            maxLength={4}
                            value={dobYear}
                            onChange={e => setDobYear(e.target.value.replace(/\D/g, ""))}
                            style={{ width: "4em" }}
                            autoComplete="bday-year"
                        />
                    </div>
                </div>
            </fieldset>

            {error && (
                <div className="nhsuk-error-message" style={{ marginBottom: "1rem" }} role="alert">
                    <strong>Error:</strong> {error}
                </div>
            )}

            <button className="nhsuk-button" type="submit" style={{ width: "100%" }}>
                Search
            </button>
        </form>
    );
};

export default SearchByDetails;