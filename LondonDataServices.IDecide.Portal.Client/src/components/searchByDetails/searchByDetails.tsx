import React, { useState } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";

interface SearchByDetailsProps {
    onBack: () => void;
    nextStep: (createdPatient: Patient) => void;
}

const isValidUKDate = (day: string, month: string, year: string): string | null => {
    if (!/^\d{2}$/.test(month) || parseInt(month, 10) < 1 || parseInt(month, 10) > 12) {
        return "Month must be between 01 and 12";
    }
    if (!/^\d{1,2}$/.test(day) || !/^\d{4}$/.test(year)) {
        return "Enter a valid date of birth";
    }
    const dayNum = parseInt(day, 10);
    const monthNum = parseInt(month, 10);
    const yearNum = parseInt(year, 10);

    if (dayNum < 1 || dayNum > 31) return "Day must be between 1 and 31";
    if (yearNum < 1900 || yearNum > new Date().getFullYear()) return "Enter a valid year";

    // Check for valid date
    const date = new Date(yearNum, monthNum - 1, dayNum);
    if (
        date.getFullYear() !== yearNum ||
        date.getMonth() !== monthNum - 1 ||
        date.getDate() !== dayNum
    ) {
        return "Enter a real date of birth";
    }
    return null;
};

const SearchByDetails: React.FC<SearchByDetailsProps> = ({ onBack, nextStep }) => {
    const [surname, setSurname] = useState("");
    const [postcode, setPostcode] = useState("");
    const [dobDay, setDobDay] = useState("");
    const [dobMonth, setDobMonth] = useState("");
    const [dobYear, setDobYear] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [loading, setLoading] = useState(false);

    const { setCreatedPatient } = useStep();
    const addPatient = patientViewService.useCreatePatient();

    const handleFieldChange = (field: string) => {
        setErrors(prev => {
            const newErrors = { ...prev };
            if (field === "dobDay" || field === "dobMonth" || field === "dobYear") {
                delete newErrors.dob;
            } else {
                delete newErrors[field];
            }
            return newErrors;
        });
    };

    const handleMonthChange = (value: string) => {
        const filtered = value.replace(/\D/g, "").slice(0, 2);
        setDobMonth(filtered);
        handleFieldChange("dobMonth");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const newErrors: { [key: string]: string } = {};
        if (!surname) newErrors.surname = "Enter your Surname";
        if (!postcode) newErrors.postcode = "Enter your Postcode";
        if (!dobDay || !dobMonth || !dobYear) {
            newErrors.dob = "Enter your Date of Birth";
        } else {
            const dobError = isValidUKDate(dobDay, dobMonth, dobYear);
            if (dobError) newErrors.dob = dobError;
        }
        setErrors(newErrors);
        if (Object.keys(newErrors).length === 0) {
            setLoading(true);
            const dateOfBirth = new Date(`${dobYear}-${dobMonth}-${dobDay}`);
            const patientToCreate = new Patient({
                id: "",
                surname,
                postcode,
                dateOfBirth
            });

            addPatient.mutate(patientToCreate, {
                onSuccess: (createdPatient: Patient) => {
                    setCreatedPatient(createdPatient);
                    nextStep(createdPatient);
                    setLoading(false);
                },
                onError: () => {
                    setErrors({ submit: "Failed to create patient. Please try again." });
                    setLoading(false);
                }
            });
        }
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

            <div className={`nhsuk-form-group${errors.surname ? " nhsuk-form-group--error" : ""}`}>
                <label className="nhsuk-label" htmlFor="surname">
                    Surname
                </label>
                <span className="nhsuk-hint" id="surname-hint">
                    For example, Smith or O'Neill
                </span>
                {errors.surname && (
                    <span className="nhsuk-error-message" id="surname-error">
                        <strong>Error:</strong> {errors.surname}
                    </span>
                )}
                <input
                    className={`nhsuk-input${errors.surname ? " nhsuk-input--error" : ""}`}
                    id="surname"
                    name="surname"
                    type="text"
                    autoComplete="family-name"
                    aria-describedby="surname-hint"
                    value={surname}
                    onChange={e => {
                        setSurname(e.target.value);
                        handleFieldChange("surname", e.target.value);
                    }}
                    style={{ marginBottom: "1rem" }}
                />
            </div>

            <div className={`nhsuk-form-group${errors.postcode ? " nhsuk-form-group--error" : ""}`}>
                <label className="nhsuk-label" htmlFor="postcode">
                    Postcode
                </label>
                <span className="nhsuk-hint" id="postcode-hint">
                    For example, SW1A 2AA
                </span>
                {errors.postcode && (
                    <span className="nhsuk-error-message" id="postcode-error">
                        <strong>Error:</strong> {errors.postcode}
                    </span>
                )}
                <input
                    className={`nhsuk-input${errors.postcode ? " nhsuk-input--error" : ""}`}
                    id="postcode"
                    name="postcode"
                    type="text"
                    autoComplete="postal-code"
                    aria-describedby="postcode-hint"
                    value={postcode}
                    onChange={e => {
                        setPostcode(e.target.value);
                        handleFieldChange("postcode", e.target.value);
                    }}
                    style={{ marginBottom: "1rem" }}
                />
            </div>

            <fieldset className={`nhsuk-fieldset${errors.dob ? " nhsuk-form-group--error" : ""}`} style={{ marginBottom: "1rem" }}>
                <legend className="nhsuk-fieldset__legend nhsuk-label">
                    Date of Birth
                </legend>
                <span className="nhsuk-hint" id="dob-hint">
                    For example, 31 03 1980
                </span>
                {errors.dob && (
                    <span className="nhsuk-error-message" id="dob-error">
                        <strong>Error:</strong> {errors.dob}
                    </span>
                )}
                <div className="nhsuk-date-input" id="dob" aria-describedby="dob-hint">
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-day">
                            Day
                        </label>
                        <input
                            className={`nhsuk-input nhsuk-date-input__input${errors.dob ? " nhsuk-input--error" : ""}`}
                            id="dob-day"
                            name="dob-day"
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            maxLength={2}
                            value={dobDay}
                            onChange={e => {
                                setDobDay(e.target.value.replace(/\D/g, ""));
                                handleFieldChange("dobDay", e.target.value);
                            }}
                            style={{ width: "3em" }}
                            autoComplete="bday-day"
                        />
                    </div>
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-month">
                            Month
                        </label>
                        <input
                            className={`nhsuk-input nhsuk-date-input__input${errors.dob ? " nhsuk-input--error" : ""}`}
                            id="dob-month"
                            name="dob-month"
                            type="text"
                            inputMode="numeric"
                            pattern="^(0[1-9]|1[0-2])$"
                            maxLength={2}
                            value={dobMonth}
                            onChange={e => handleMonthChange(e.target.value)}
                            style={{ width: "3em" }}
                            autoComplete="bday-month"
                        />
                    </div>
                    <div className="nhsuk-date-input__item" style={{ display: "inline-block" }}>
                        <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-year">
                            Year
                        </label>
                        <input
                            className={`nhsuk-input nhsuk-date-input__input${errors.dob ? " nhsuk-input--error" : ""}`}
                            id="dob-year"
                            name="dob-year"
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            maxLength={4}
                            value={dobYear}
                            onChange={e => {
                                setDobYear(e.target.value.replace(/\D/g, ""));
                                handleFieldChange("dobYear", e.target.value);
                            }}
                            style={{ width: "4em" }}
                            autoComplete="bday-year"
                        />
                    </div>
                </div>
            </fieldset>

            {errors.submit && (
                <div className="nhsuk-error-message" style={{ marginBottom: "1rem" }} role="alert">
                    <strong>Error:</strong> {errors.submit}
                </div>
            )}

            <button className="nhsuk-button" type="submit" style={{ width: "100%" }} disabled={loading}>
                {loading ? "Submitting..." : "Search"}
            </button>
        </form>
    );
};

export default SearchByDetails;