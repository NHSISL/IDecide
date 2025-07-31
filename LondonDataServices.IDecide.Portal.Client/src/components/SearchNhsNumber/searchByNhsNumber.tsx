import React, { useState, useEffect } from "react";
import { useStep } from "../context/stepContext";
import { Patient } from "../../models/patients/patient";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button, Select } from "nhsuk-react-components";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { Container, Row, Col } from "react-bootstrap";

const RECAPTCHA_SITE_KEY = "6LcOJn4rAAAAAIUdB70R9BqkfPFD-bPYTk6ojRGg";

export const SearchByNhsNumber = ({
    onIDontKnow,
    powerOfAttourney = false
}: {
    onIDontKnow: () => void;
    powerOfAttourney?: boolean;
}) => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [poaNhsNumberInput, setPoaNhsNumberInput] = useState("");
    const [poaFirstname, setPoaFirstname] = useState("");
    const [poaSurname, setPoaSurname] = useState("");
    const [poaRelationship, setPoaRelationship] = useState("");
    const [error, setError] = useState("");
    const [poaNhsNumberError, setPoaNhsNumberError] = useState("");
    const [poaFirstnameError, setPoaFirstnameError] = useState("");
    const [poaSurnameError, setPoaSurnameError] = useState("");
    const [poaRelationshipError, setPoaRelationshipError] = useState("");
    const [loading, setLoading] = useState(false);
    const [recaptchaReady, setRecaptchaReady] = useState(false);
    const { nextStep, setCreatedPatient } = useStep();
    const addPatient = patientViewService.useCreatePatient();

    useEffect(() => {
        let isMounted = true;
        loadRecaptchaScript(RECAPTCHA_SITE_KEY)
            .then(() => {
                const waitForGrecaptcha = () => {
                    if (window.grecaptcha && typeof window.grecaptcha.ready === "function") {
                        window.grecaptcha.ready(() => {
                            if (isMounted) setRecaptchaReady(true);
                        });
                    } else {
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

    // PoA field handlers
    const handlePoaNhsNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setPoaNhsNumberInput(value);
        setPoaNhsNumberError("");
    };
    const handlePoaFirstnameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPoaFirstname(e.target.value);
        setPoaFirstnameError("");
    };
    const handlePoaSurnameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPoaSurname(e.target.value);
        setPoaSurnameError("");
    };
    const handlePoaRelationshipChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setPoaRelationship(e.target.value);
        setPoaRelationshipError("");
    };

    const validatePoaFields = () => {
        let valid = true;
        if (poaNhsNumberInput.length !== 10) {
            setPoaNhsNumberError("Enter a 10-digit NHS Number");
            valid = false;
        }
        if (!poaFirstname.trim()) {
            setPoaFirstnameError("Enter a first name");
            valid = false;
        }
        if (!poaSurname.trim()) {
            setPoaSurnameError("Enter a surname");
            valid = false;
        }
        if (!poaRelationship) {
            setPoaRelationshipError("Select a relationship");
            valid = false;
        }
        return valid;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setPoaNhsNumberError("");
        setPoaFirstnameError("");
        setPoaSurnameError("");
        setPoaRelationshipError("");

        if (powerOfAttourney) {
            if (!validatePoaFields()) return;
        } else {
            if (nhsNumberInput.length !== 10) {
                setError("Enter a 10-digit NHS Number");
                return;
            }
        }

        if (!recaptchaReady || typeof grecaptcha === "undefined") {
            setError("reCAPTCHA is not ready. Please try again later.");
            return;
        }
        setLoading(true);
        try {
            grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: "submit" }).then((token: string) => {
                const nhsNumberToUse = powerOfAttourney ? poaNhsNumberInput : nhsNumberInput;
                const patientToCreate = new Patient({ id: "", nhsNumber: nhsNumberToUse, recaptchaToken: token });
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
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
                    <form autoComplete="off" onSubmit={handleSubmit}>
                        {!powerOfAttourney && (
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
                                error={error || undefined}
                                style={{ maxWidth: "200px" }}
                            />
                        )}

                        {powerOfAttourney && (
                            <div style={{ marginBottom: "1.5rem" }}>
                                <TextInput
                                    label="NHS Number of the person you are representing"
                                    id="poa-nhs-number"
                                    name="poa-nhs-number"
                                    inputMode="numeric"
                                    pattern="\d*"
                                    maxLength={10}
                                    autoComplete="off"
                                    value={poaNhsNumberInput}
                                    onChange={handlePoaNhsNumberChange}
                                    error={poaNhsNumberError || undefined}
                                    style={{ maxWidth: "200px", marginBottom: "1rem" }}
                                />
                                <TextInput
                                    label="Firstname"
                                    id="poa-firstname"
                                    name="poa-firstname"
                                    autoComplete="off"
                                    value={poaFirstname}
                                    onChange={handlePoaFirstnameChange}
                                    error={poaFirstnameError || undefined}
                                    style={{ maxWidth: "300px", marginBottom: "1rem" }}
                                />
                                <TextInput
                                    label="Surname"
                                    id="poa-surname"
                                    name="poa-surname"
                                    autoComplete="off"
                                    value={poaSurname}
                                    onChange={handlePoaSurnameChange}
                                    error={poaSurnameError || undefined}
                                    style={{ maxWidth: "300px", marginBottom: "1rem" }}
                                />
                                <div style={{ marginBottom: "1rem" }}>
                                    <Select
                                        label="Relationship"
                                        id="poa-relationship"
                                        name="poa-relationship"
                                        aria-label="Relationship to the person you are representing"
                                        aria-required="true"
                                        required
                                        value={poaRelationship}
                                        onChange={handlePoaRelationshipChange}
                                        error={poaRelationshipError || undefined}
                                        style={{ maxWidth: "300px", marginBottom: "1rem" }}
                                    >
                                        <option value="" disabled>
                                            Select relationship
                                        </option>
                                        <option value="Parent">Parent</option>
                                        <option value="Carer">Carer</option>
                                    </Select>
                                </div>
                            </div>
                        )}

                        <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem", marginTop: "1rem" }}>
                            <Button
                                type="submit"
                                disabled={
                                    loading ||
                                    !recaptchaReady ||
                                    (powerOfAttourney
                                        ? !poaNhsNumberInput ||
                                          !poaFirstname.trim() ||
                                          !poaSurname.trim() ||
                                          !poaRelationship ||
                                          poaNhsNumberInput.length !== 10
                                        : nhsNumberInput.length !== 10)
                                }
                            >
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
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                    {powerOfAttourney && (
                        <div
                            className="p-4 mb-4"
                            style={{
                                background: "#f4f8fb",
                                border: "1px solid #d1e3f0",
                                borderRadius: "8px",
                                boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                            }}
                        >
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>Help & Guidance</h2>
                            <p>
                                <strong>Why we ask for Power of Attorney details</strong><br />
                                If you are acting on behalf of someone else, we need to confirm your authority to do so. Providing your Power of Attorney details helps us ensure that you have the legal right to make decisions or access information for the person you represent.
                            </p>
                            <p>
                                <strong>What to enter:</strong>
                                <ul>
                                    <li><strong>First name and surname:</strong> Enter your own name as it appears on your Power of Attorney documentation.</li>
                                    <li><strong>Relationship:</strong> Select your relationship to the person you are representing (for example, Parent or Carer).</li>
                                </ul>
                            </p>
                            <p>
                                <strong>What happens next:</strong>
                                <ul>
                                    <li>The information you provide will be used to verify your authority to act on behalf of the individual.</li>
                                    <li>We may contact you or request further documentation if needed.</li>
                                </ul>
                            </p>
                            <p className="mb-2">
                                <strong>Important:</strong> Please ensure the details you provide match your official Power of Attorney documents. If you are unsure about your status or need more information, you can find guidance on the&nbsp;
                                <a href="https://www.gov.uk/power-of-attorney" target="_blank" rel="noopener noreferrer">
                                    UK Government website about Power of Attorney
                                </a>.
                            </p>
                        </div>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default SearchByNhsNumber;