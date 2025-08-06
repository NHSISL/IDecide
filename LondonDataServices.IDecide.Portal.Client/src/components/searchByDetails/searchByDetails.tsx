import React, { useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Select, Card } from "nhsuk-react-components";
import { Col, Container, Row } from "react-bootstrap";

interface SearchByDetailsProps {
    onBack: () => void;
    powerOfAttourney?: boolean;
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

const SearchByDetails: React.FC<SearchByDetailsProps> = ({ onBack, powerOfAttourney }) => {
    // Standard fields
    const [surname, setSurname] = useState("");
    const [postcode, setPostcode] = useState("");
    const [dobDay, setDobDay] = useState("");
    const [dobMonth, setDobMonth] = useState("");
    const [dobYear, setDobYear] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [loading, setLoading] = useState(false);

    // PoA fields (NHS Number removed)
    const [poaFirstname, setPoaFirstname] = useState("");
    const [poaSurname, setPoaSurname] = useState("");
    const [poaRelationship, setPoaRelationship] = useState("");
    const [poaFirstnameError, setPoaFirstnameError] = useState("");
    const [poaSurnameError, setPoaSurnameError] = useState("");
    const [poaRelationshipError, setPoaRelationshipError] = useState("");

    const { nextStep, setCreatedPatient } = useStep();
    const addPatient = patientViewService.usePostPatientDetails();

    // PoA handlers
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

    // Standard field error clearing
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

        // Standard validation
        if (!surname) newErrors.surname = "Enter your Surname";
        if (!postcode) newErrors.postcode = "Enter your Postcode";
        if (!dobDay || !dobMonth || !dobYear) {
            newErrors.dob = "Enter your Date of Birth";
        } else {
            const dobError = isValidUKDate(dobDay, dobMonth, dobYear);
            if (dobError) newErrors.dob = dobError;
        }

        // PoA validation
        if (powerOfAttourney) {
            if (!poaFirstname.trim()) newErrors.poaFirstname = "Enter a first name";
            if (!poaSurname.trim()) newErrors.poaSurname = "Enter a surname";
            if (!poaRelationship) newErrors.poaRelationship = "Select a relationship";
        }

        setErrors(newErrors);

        if (Object.keys(newErrors).length === 0) {
            setLoading(true);
            const dateOfBirth = new Date(`${dobYear}-${dobMonth}-${dobDay}`);
            const patientToCreate = new Patient({
                id: "",
                surname,
                postcode,
                dateOfBirth,
            });

            let poaModel = undefined;
            if (powerOfAttourney) {
                poaModel = new PowerOfAttourney({
                    firstName: poaFirstname,
                    surname: poaSurname,
                    relationship: poaRelationship
                });
            }

            addPatient.mutate(patientToCreate, {
                onSuccess: (createdPatient: Patient) => {
                    setCreatedPatient(createdPatient);
                    nextStep(undefined, undefined, createdPatient, poaModel);
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
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
                    <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit} >
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

                        <Card cardType="feature">
                            <Card.Content>
                                <Card.Heading>My Details</Card.Heading>

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
                                            handleFieldChange("surname");
                                        }}
                                        style={{ marginBottom: "1rem", maxWidth: "400px" }}
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
                                            handleFieldChange("postcode");
                                        }}
                                        style={{ marginBottom: "1rem", maxWidth: "400px" }}
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
                                                    handleFieldChange("dobDay");
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
                                                    handleFieldChange("dobYear");
                                                }}
                                                style={{ width: "4em" }}
                                                autoComplete="bday-year"
                                            />
                                        </div>
                                    </div>
                                </fieldset>
                            </Card.Content>
                        </Card>

                        {powerOfAttourney && (
                            <Card cardType="feature">
                                <Card.Content>
                                    <Card.Heading>My Details (The Requester)</Card.Heading>
                                    <Card.Description>
                                        <div style={{ marginBottom: "1.5rem" }}>
                                            <TextInput
                                                label="Firstname"
                                                id="poa-firstname"
                                                name="poa-firstname"
                                                autoComplete="off"
                                                value={poaFirstname}
                                                onChange={handlePoaFirstnameChange}
                                                error={poaFirstnameError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            />
                                            <TextInput
                                                label="Surname"
                                                id="poa-surname"
                                                name="poa-surname"
                                                autoComplete="off"
                                                value={poaSurname}
                                                onChange={handlePoaSurnameChange}
                                                error={poaSurnameError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
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
                                                    style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                                >
                                                    <option value="" disabled>
                                                        Select relationship
                                                    </option>
                                                    <option value="The patient is under 13 and you are their parent">
                                                        The patient is under 13 and you are their parent
                                                    </option>
                                                    <option value="The patient is under 13 and you are their appointed guardian">
                                                        The patient is under 13 and you are their appointed guardian
                                                    </option>
                                                    <option value="The patient is over 13 and you have power of attorney with the right to act on their behalf.">
                                                        The patient is over 13 and you have power of attorney with the right to act on their behalf.
                                                    </option>
                                                </Select>
                                            </div>
                                        </div>
                                    </Card.Description>
                                </Card.Content>
                            </Card>
                        )}

                        {errors.submit && (
                            <div className="nhsuk-error-message" style={{ marginBottom: "1rem" }} role="alert">
                                <strong>Error:</strong> {errors.submit}
                            </div>
                        )}

                        <button className="nhsuk-button" type="submit" style={{ width: "100%" }} disabled={loading}>
                            {loading ? "Submitting..." : "Search"}
                        </button>
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
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                Requesting an Opt-out on someone else's behalf
                            </h3>
                            <p>
                                You can make a request to opt-out on behalf of someone else to stop their personal data being used for secondary purposes if:
                            </p>
                            <ul>
                                <li>The patient is under 13 and you are their parent</li>
                                <li>The patient is under 13 and you are their appointed guardian</li>
                                <li>The patient is over 13 and you have power of attorney with the right to act on their behalf.</li>
                            </ul>
                            <p>
                                If you are in these circumstances then please enter your details in this blue box and in every other box use the patient's details.
                            </p>
                            <p>
                                If one of these circumstances does not describe you then you cannot opt someone else out. Please click the back button.
                            </p>
                        </div>
                    )}
                </Col>
            </Row>
        </Container >
    );
};

export default SearchByDetails;