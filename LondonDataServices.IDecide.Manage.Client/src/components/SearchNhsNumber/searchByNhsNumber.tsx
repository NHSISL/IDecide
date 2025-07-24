import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

export const SearchByNhsNumber = () => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setNhsNumberInput(value);
        if (error) setError("");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (nhsNumberInput.length !== 10) {
            setError("NHS Number must be exactly 10 digits.");
            return;
        }

        // Navigate to the confirm details page
        navigate("/confirmDetails");
    };

    return (
        <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit}>
            <label className="nhsuk-label" htmlFor="nhs-number">
                NHS Number
            </label>
            <input
                className="nhsuk-input"
                id="nhs-number"
                name="nhs-number"
                type="text"
                inputMode="numeric"
                pattern="\d*"
                maxLength={10}
                autoComplete="off"
                value={nhsNumberInput}
                onChange={handleInputChange}
                style={{ maxWidth: "200px" }}
                aria-describedby={error ? "nhs-number-error" : undefined}
            />
            {error && (
                <span
                    id="nhs-number-error"
                    className="nhsuk-error-message"
                    style={{ display: "block", marginTop: "0.5rem" }}
                >
                    <strong>Error:</strong> {error} Only digits are allowed and the length must be 10.
                </span>
            )}

            <div style={{ marginTop: "1.5rem" }}>
                <button className="nhsuk-button" type="submit">
                    Search
                </button>
            </div>
        </form>
    );
};

export default SearchByNhsNumber;
