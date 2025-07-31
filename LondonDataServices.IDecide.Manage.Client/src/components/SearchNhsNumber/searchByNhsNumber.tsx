import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { TextInput, Button } from "nhsuk-react-components";
import { Container, Row, Col } from "react-bootstrap";

export const SearchByNhsNumber = () => {
    const [nhsNumberInput, setNhsNumberInput] = useState("1234567890");
    const [error, setError] = useState("");
    const [isPowerOfAttorney, setIsPowerOfAttorney] = useState(false);
    const navigate = useNavigate();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setNhsNumberInput(value);
        if (error) setError("");
    };

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIsPowerOfAttorney(e.target.checked);
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
        <Container fluid>
            <Row className="justify-content-center">
                <Col md={6} lg={7} xl={6}>
                    <form autoComplete="off" onSubmit={handleSubmit}>
                        <div style={{ margin: "1rem 0" }}>
                            <label>
                                <input
                                    type="checkbox"
                                    checked={isPowerOfAttorney}
                                    onChange={handleCheckboxChange}
                                    style={{ marginRight: "0.5rem" }}
                                />
                                I am acting under a Power of Attorney
                            </label>
                        </div>

                        {isPowerOfAttorney ? (
                            <div
                                style={{marginBottom: "1rem" }}
                                data-testid="power-of-attorney-section"
                            >
                                
                                <p>
                                    Please provide details and documentation regarding your Power of Attorney status.
                                </p>
                                <TextInput
                                    label="NHS Number"
                                    hint="It's on your National Insurance card, benefit letter, payslip or P60."
                                    id="nhs-number-poa"
                                    name="nhs-number-poa"
                                    inputMode="numeric"
                                    pattern="\d*"
                                    maxLength={10}
                                    autoComplete="off"
                                    value={nhsNumberInput}
                                    onChange={handleInputChange}
                                    error={error ? "NHS Number must be exactly 10 digits. Only digits are allowed." : undefined}
                                    style={{ maxWidth: "200px" }}
                                />
                            </div>
                        ) : (
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
                        )}

                        <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem", marginTop: "1rem" }}>
                            <Button type="submit">Search</Button>
                        </div>
                    </form>
                </Col>
                <Col md={6} lg={5} xl={6}>
                    {isPowerOfAttorney && (
                        <aside
                            style={{
                                background: "#f8f8f8",
                                border: "1px solid #e5e5e5",
                                borderRadius: "4px",
                                padding: "1.5rem",
                                minWidth: "250px",
                                marginTop: "3.6rem"
                            }}>

                            <h3>Help and Guidance</h3>
                            <p>
                                Your NHS Number is a 10-digit number, like 485 777 3456. You can find it on any letter the NHS has sent you, on a prescription, or by logging in to a GP online service. If you cannot find your NHS Number, contact your GP practice.
                            </p>
                            <ul>
                                <li>Only enter numbers, no spaces or letters.</li>
                                <li>If you are acting under a Power of Attorney, please tick the box and provide the required details.</li>
                            </ul>
                        </aside>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default SearchByNhsNumber;