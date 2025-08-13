import React, { useContext, useEffect, useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Select, Card } from "nhsuk-react-components";
import { Col, Container, Row } from "react-bootstrap";
import { StepContext } from "../context/stepContext";
import { useTranslation } from "react-i18next";

interface SearchByDetailsProps {
    onBack: () => void;
    powerOfAttourney?: boolean;
}

const SearchByDetails: React.FC<SearchByDetailsProps> = ({ onBack, powerOfAttourney }) => {
    const { t } = useTranslation();
    const stepContext = useContext(StepContext);

    useEffect(() => {
        if (stepContext && typeof stepContext.resetStepContext === "function") {
            stepContext.resetStepContext();
        }
    }, [stepContext]);

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

    const isValidUKDate = (day: string, month: string, year: string): string | null => {
        if (!/^\d{2}$/.test(month) || parseInt(month, 10) < 1 || parseInt(month, 10) > 12) {
            return t("SearchByDetails.dobMonthInvalid");
        }
        if (!/^\d{1,2}$/.test(day) || !/^\d{4}$/.test(year)) {
            return t("SearchByDetails.dobInvalid");
        }
        const dayNum = parseInt(day, 10);
        const monthNum = parseInt(month, 10);
        const yearNum = parseInt(year, 10);

        if (dayNum < 1 || dayNum > 31) return t("SearchByDetails.dobDayInvalid");
        if (yearNum < 1900 || yearNum > new Date().getFullYear()) return t("SearchByDetails.dobYearInvalid");

        // Check for valid date
        const date = new Date(yearNum, monthNum - 1, dayNum);
        if (
            date.getFullYear() !== yearNum ||
            date.getMonth() !== monthNum - 1 ||
            date.getDate() !== dayNum
        ) {
            return t("SearchByDetails.dobRealInvalid");
        }
        return null;
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
        if (!surname) newErrors.surname = t("SearchByDetails.surnameError");
        if (!postcode) newErrors.postcode = t("SearchByDetails.postcodeError");
        if (!dobDay || !dobMonth || !dobYear) {
            newErrors.dob = t("SearchByDetails.dobError");
        } else {
            const dobError = isValidUKDate(dobDay, dobMonth, dobYear);
            if (dobError) newErrors.dob = dobError;
        }

        // PoA validation
        if (powerOfAttourney) {
            if (!poaFirstname.trim()) newErrors.poaFirstname = t("SearchByDetails.poaFirstnameError");
            if (!poaSurname.trim()) newErrors.poaSurname = t("SearchByDetails.poaSurnameError");
            if (!poaRelationship) newErrors.poaRelationship = t("SearchByDetails.poaRelationshipError");
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
                    setErrors({ submit: t("SearchByDetails.submitError") });
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
                            aria-label={t("SearchByDetails.back")}
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
                            {t("SearchByDetails.back")}
                        </button>

                        <Card cardType="feature">
                            <Card.Content>
                                <Card.Heading>{t("SearchByDetails.myDetails")}</Card.Heading>

                                <div className={`nhsuk-form-group${errors.surname ? " nhsuk-form-group--error" : ""}`}>
                                    <label className="nhsuk-label" htmlFor="surname">
                                        {t("SearchByDetails.surnameLabel")}
                                    </label>
                                    <span className="nhsuk-hint" id="surname-hint">
                                        {t("SearchByDetails.surnameHint")}
                                    </span>
                                    {errors.surname && (
                                        <span className="nhsuk-error-message" id="surname-error">
                                            <strong>{t("SearchByDetails.errorPrefix")}</strong> {errors.surname}
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
                                        {t("SearchByDetails.postcodeLabel")}
                                    </label>
                                    <span className="nhsuk-hint" id="postcode-hint">
                                        {t("SearchByDetails.postcodeHint")}
                                    </span>
                                    {errors.postcode && (
                                        <span className="nhsuk-error-message" id="postcode-error">
                                            <strong>{t("SearchByDetails.errorPrefix")}</strong> {errors.postcode}
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
                                        {t("SearchByDetails.dobLegend")}
                                    </legend>
                                    <span className="nhsuk-hint" id="dob-hint">
                                        {t("SearchByDetails.dobHint")}
                                    </span>
                                    {errors.dob && (
                                        <span className="nhsuk-error-message" id="dob-error">
                                            <strong>{t("SearchByDetails.errorPrefix")}</strong> {errors.dob}
                                        </span>
                                    )}
                                    <div className="nhsuk-date-input" id="dob" aria-describedby="dob-hint">
                                        <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                                            <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-day">
                                                {t("SearchByDetails.dobDayLabel")}
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
                                                {t("SearchByDetails.dobMonthLabel")}
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
                                                {t("SearchByDetails.dobYearLabel")}
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
                                    <Card.Heading>{t("SearchByDetails.myDetailsRequester")}</Card.Heading>
                                    <Card.Description>
                                        <div style={{ marginBottom: "1.5rem" }}>
                                            <TextInput
                                                label={t("SearchByDetails.poaFirstnameLabel")}
                                                id="poa-firstname"
                                                name="poa-firstname"
                                                autoComplete="off"
                                                value={poaFirstname}
                                                onChange={handlePoaFirstnameChange}
                                                error={poaFirstnameError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            />
                                            <TextInput
                                                label={t("SearchByDetails.poaSurnameLabel")}
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
                                                    label={t("SearchByDetails.poaRelationshipLabel")}
                                                    id="poa-relationship"
                                                    name="poa-relationship"
                                                    aria-label={t("SearchByDetails.poaRelationshipLabel")}
                                                    aria-required="true"
                                                    required
                                                    value={poaRelationship}
                                                    onChange={handlePoaRelationshipChange}
                                                    error={poaRelationshipError || undefined}
                                                    style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                                >
                                                    <option value="" disabled>
                                                        {t("SearchByDetails.poaRelationshipSelect")}
                                                    </option>
                                                    <option value={t("SearchByDetails.poaRelationshipParent")}>
                                                        {t("SearchByDetails.poaRelationshipParent")}
                                                    </option>
                                                    <option value={t("SearchByDetails.poaRelationshipGuardian")}>
                                                        {t("SearchByDetails.poaRelationshipGuardian")}
                                                    </option>
                                                    <option value={t("SearchByDetails.poaRelationshipAttorney")}>
                                                        {t("SearchByDetails.poaRelationshipAttorney")}
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
                                <strong>{t("SearchByDetails.errorPrefix")}</strong> {errors.submit}
                            </div>
                        )}

                        <button className="nhsuk-button" type="submit" style={{ width: "100%" }} disabled={loading}>
                            {loading ? t("SearchByDetails.submitting") : t("SearchByDetails.searchButton")}
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
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{t("SearchByDetails.helpGuidanceTitle")}</h2>
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                {t("SearchByDetails.helpGuidanceSubtitle")}
                            </h3>
                            <p>
                                {t("SearchByDetails.helpGuidanceText1")}
                            </p>
                            <ul>
                                <li>{t("SearchByDetails.poaRelationshipParent")}</li>
                                <li>{t("SearchByDetails.poaRelationshipGuardian")}</li>
                                <li>{t("SearchByDetails.poaRelationshipAttorney")}</li>
                            </ul>
                            <p>
                                {t("SearchByDetails.helpGuidanceText2")}
                            </p>
                            <p>
                                {t("SearchByDetails.helpGuidanceText3")}
                            </p>
                        </div>
                    )}
                </Col>
            </Row>
        </Container >
    );
};

export default SearchByDetails;