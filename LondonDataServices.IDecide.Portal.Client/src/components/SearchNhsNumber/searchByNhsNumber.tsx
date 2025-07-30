import React, { useState, useEffect } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button } from "nhsuk-react-components";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";

const RECAPTCHA_SITE_KEY = "6LcOJn4rAAAAAIUdB70R9BqkfPFD-bPYTk6ojRGg";

export const SearchByNhsNumber = ({ onIDontKnow }: { onIDontKnow: () => void }) => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [recaptchaReady, setRecaptchaReady] = useState(false);
    const { nextStep, setCreatedPatient } = useStep();
    const addPatient = patientViewService.useCreatePatient();

    useEffect(() => {
        let isMounted = true;
        loadRecaptchaScript(RECAPTCHA_SITE_KEY)
            .then(() => {
                // Wait for grecaptcha to be available
                const waitForGrecaptcha = () => {
                    if (window.grecaptcha && typeof window.grecaptcha.ready === "function") {
                        window.grecaptcha.ready(() => {
                            if (isMounted) setRecaptchaReady(true);
                        });
                    } else {
                        // Try again in 50ms
                        setTimeout(waitForGrecaptcha, 50);
                    }
                };
                waitForGrecaptcha();
            })
            .catch(() => {
                if (isMounted) setError("Failed to load reCAPTCHA. Please refresh and try again.");
            });
        return () => { isMounted = false; };
    }, []);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setNhsNumberInput(value);
        if (error) setError("");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (nhsNumberInput.length !== 10) {
            setError("NHS Number must be exactly 10 digits.");
            return;
        }
        if (!recaptchaReady || typeof grecaptcha === "undefined") {
            setError("reCAPTCHA is not ready. Please try again later.");
            return;
        }
        setLoading(true);
        try {
            grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: "submit" }).then((token: string) => {
                const patientToCreate = new Patient({ id: "", nhsNumber: nhsNumberInput, recaptchaToken: token });
                addPatient.mutate(patientToCreate, {
                    onSuccess: (createdPatient) => {
                        setCreatedPatient(createdPatient);
                        nextStep(undefined, undefined, createdPatient);
                        setLoading(false);
                    },
                    onError: () => {
                        setError("Failed to create patient. Please try again.");
                        setLoading(false);
                    }
                });
            });
        } catch {
            setError("reCAPTCHA failed. Please try again.");
            setLoading(false);
        }
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
                <Button type="submit" disabled={loading || !recaptchaReady || nhsNumberInput.length !== 10}>
                    {loading ? "Submitting..." : "Search"}
                </Button>
                <Button
                    type="button"
                    secondary
                    onClick={onIDontKnow}
                    disabled={loading}
                >
                    I Don't know my NHS Number
                </Button>
            </div>
        </form>
    );
};

export default SearchByNhsNumber;