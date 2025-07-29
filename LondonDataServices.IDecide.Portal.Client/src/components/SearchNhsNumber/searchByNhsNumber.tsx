import React, { useState } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button } from "nhsuk-react-components";

export const SearchByNhsNumber = ({ onIDontKnow }: { onIDontKnow: () => void }) => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [error, setError] = useState("");
    const { nextStep, setCreatedPatient } = useStep();

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

        const patientToCreate = new Patient({ id: "", nhsNumber: nhsNumberInput });

        addPatient.mutate(patientToCreate, {
            onSuccess: (createdPatient) => {
                setCreatedPatient(createdPatient);
                nextStep(createdPatient);
            },
            onError: (error: any) => {
                // Optionally set error here
            }
        });
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
                <Button
                    type="button"
                    secondary
                    onClick={onIDontKnow}
                >
                    I Don't know my NHS Number
                </Button>
            </div>
        </form>
    );
};

export default SearchByNhsNumber;