import React, { useState } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";

export const SearchByNhsNumber = ({ onIDontKnow }: { onIDontKnow: () => void }) => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [error, setError] = useState("");
    const { nextStep, setNhsNumber } = useStep();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setNhsNumberInput(value);
        if (error) setError("");
    };

    const addPatient = patientViewService.useCreatePatient();

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (nhsNumberInput.length !== 10) {
            setError("NHS Number must be exactly 10 digits.");
            return;
        }
        setNhsNumber(nhsNumberInput);

        const patientToCreate = new Patient({ id: "", nhsNumber: nhsNumberInput });

        addPatient.mutate(patientToCreate, {
            onSuccess: (createdPatient) => {
                console.log("Created patient:", createdPatient);
                nextStep();
            },
            onError: (error: any) => {
                console.error("Error creating patient:", error);
            }
        });
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
                <button
                    className="nhsuk-button nhsuk-button--secondary"
                    type="button"
                    style={{ marginLeft: "1rem" }}
                    onClick={onIDontKnow}
                >
                    I Don't know my NHS Number
                </button>
            </div>
        </form>
    );
};

export default SearchByNhsNumber;