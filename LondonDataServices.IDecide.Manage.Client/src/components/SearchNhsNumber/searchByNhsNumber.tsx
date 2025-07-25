import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { TextInput, Button } from "nhsuk-react-components";

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
        navigate("/confirmDetails");
    };

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <TextInput
                label="NHS Number"
                hint="It's on your National Insurance card, benefit letter, payslip or P60."
                id="nhs-number"
                name="nhs-number"
                inputMode="numeric"
                pattern="\d*"
                maxLength={10}
                autoComplete="off"
                value={nhsNumberInput}
                onChange={handleInputChange}
                error={error ? "NHS Number must be exactly 10 digits. Only digits are allowed." : undefined}
                style={{ maxWidth: "200px" }}
            />

            <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem", marginTop: "1rem" }}>
                <Button type="submit">Search</Button>
                
            </div>
        </form>
    );
};

export default SearchByNhsNumber;