import React, { useState } from "react";
import { useStep } from "../context/stepContext";

export const ConfirmCode = () => {
    const [code, setCode] = useState("12345");
    const [error, setError] = useState("");
    const { nextStep } = useStep();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 5);
        setCode(value);
        if (error) setError("");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (code.length !== 5) {
            setError("Please enter a 5-digit code.");
            return;
        }
        nextStep();
    };

    return (
        <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit} >
            <label className="nhsuk-label" htmlFor="code">
                Enter Code
            </label>
            <input
                className="nhsuk-input"
                id="code"
                name="code"
                type="text"
                inputMode="numeric"
                pattern="\d{5}"
                maxLength={5}
                autoComplete="one-time-code"
                value={code}
                onChange={handleInputChange}
                style={{ width: "100%", maxWidth: "200px" }}
                aria-describedby={error ? "code-error" : undefined}
                aria-invalid={!!error}
            />
            {error && (
                <div
                    id="code-error"
                    className="nhsuk-error-message"
                    style={{ marginTop: "0.5rem" }}
                    role="alert"
                >
                    <strong>Error:</strong> {error}
                </div>
            )}

            <button className="nhsuk-button" type="submit" style={{ width: "100%", marginTop: "1.5rem" }}>
                Submit
            </button>
        </form>
    );
};

export default ConfirmCode;