import React, { useState } from "react";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Select, Card } from "nhsuk-react-components";
import { Col, Container, Row } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { PatientLookup } from "../../models/patients/patientLookup";
import { SearchCriteria } from "../../models/searchCriterias/searchCriteria";
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { useNavigate } from "react-router-dom";

export const SearchByDetails = () => {
    const { t: translate } = useTranslation();
    const [surname, setSurname] = useState("");
    const [postcode, setPostcode] = useState("");
    const [dobDay, setDobDay] = useState("");
    const [dobMonth, setDobMonth] = useState("");
    const [dobYear, setDobYear] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [loading, setLoading] = useState(false);
    const [isPowerOfAttorney, setIsPowerOfAttorney] = useState(false);
    const [poaFirstname, setPoaFirstname] = useState("");
    const [poaSurname, setPoaSurname] = useState("");
    const [poaRelationship, setPoaRelationship] = useState("");
    const addPatient = patientViewService.usePostPatientDetails();
    const navigate = useNavigate();

    // PoA handlers
    const handlePoaFirstnameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPoaFirstname(e.target.value);
        handleFieldChange("poaFirstname");
    };
    const handlePoaSurnameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPoaSurname(e.target.value);
        handleFieldChange("poaSurname");
    };
    const handlePoaRelationshipChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setPoaRelationship(e.target.value);
        handleFieldChange("poaRelationship");
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

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIsPowerOfAttorney(e.target.checked);
    };

    // Normalize and validate UK date
    const isValidUKDate = (day: string, month: string, year: string): string | null => {
        // Normalize to two digits for day and month
        const dayNorm = day.padStart(2, "0");
        const monthNorm = month.padStart(2, "0");

        if (!/^(0[1-9]|[12][0-9]|3[01])$/.test(dayNorm)) {
            return translate("SearchByDetails.dobDayInvalid");
        }
        if (!/^(0[1-9]|1[0-2])$/.test(monthNorm)) {
            return translate("SearchByDetails.dobMonthInvalid");
        }
        if (!/^\d{4}$/.test(year)) {
            return translate("SearchByDetails.dobYearInvalid");
        }
        const dayNum = parseInt(dayNorm, 10);
        const monthNum = parseInt(monthNorm, 10);
        const yearNum = parseInt(year, 10);

        if (yearNum < 1900 || yearNum > new Date().getFullYear()) {
            return translate("SearchByDetails.dobYearInvalid");
        }

        // Check for valid date
        const date = new Date(yearNum, monthNum - 1, dayNum);
        if (
            date.getFullYear() !== yearNum ||
            date.getMonth() !== monthNum - 1 ||
            date.getDate() !== dayNum
        ) {
            return translate("SearchByDetails.dobRealInvalid");
        }
        return null;
    };

    const handleMonthChange = (value: string) => {
        // Only allow 1-12, two digits max
        let filtered = value.replace(/\D/g, "").slice(0, 2);
        if (filtered.length === 2 && (parseInt(filtered, 10) < 1 || parseInt(filtered, 10) > 12)) {
            filtered = filtered[0];
        }
        setDobMonth(filtered);
        handleFieldChange("dobMonth");
    };

    const handleDayChange = (value: string) => {
        // Only allow 1-31, two digits max
        let filtered = value.replace(/\D/g, "").slice(0, 2);
        if (filtered.length === 2 && (parseInt(filtered, 10) < 1 || parseInt(filtered, 10) > 31)) {
            filtered = filtered[0];
        }
        setDobDay(filtered);
        handleFieldChange("dobDay");
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const newErrors: { [key: string]: string } = {};

        // Standard validation
        if (!surname) newErrors.surname = translate("SearchByDetails.surnameError");
        if (!postcode) newErrors.postcode = translate("SearchByDetails.postcodeError");
        if (!dobDay || !dobMonth || !dobYear) {
            newErrors.dob = translate("SearchByDetails.dobError");
        } else {
            const dobError = isValidUKDate(dobDay, dobMonth, dobYear);
            if (dobError) newErrors.dob = dobError;
        }

        // PoA validation
        if (isPowerOfAttorney) {
            if (!poaFirstname.trim()) newErrors.poaFirstname = translate("SearchByDetails.poaFirstnameError");
            if (!poaSurname.trim()) newErrors.poaSurname = translate("SearchByDetails.poaSurnameError");
            if (!poaRelationship) newErrors.poaRelationship = translate("SearchByDetails.poaRelationshipError");
        }

        setErrors(newErrors);

        if (Object.keys(newErrors).length === 0) {
            setLoading(true);
            const dateOfBirth = `${dobYear}/${dobMonth.padStart(2, "0")}/${dobDay.padStart(2, "0")}`;
            const searchCriteria = new SearchCriteria({
                surname: surname,
                postcode: postcode,
                dateOfBirth: dateOfBirth.toString()
            });
            const patientLookup = new PatientLookup(searchCriteria, []);

            let poaModel = undefined;
            if (isPowerOfAttorney) {
                poaModel = new PowerOfAttourney({
                    firstName: poaFirstname,
                    surname: poaSurname,
                    relationship: poaRelationship
                });
            }

            addPatient.mutate(
                patientLookup,
                {
                    onSuccess: (createdPatient: Patient) => {
                        navigate("/confirmDetails", { state: { createdPatient, poaModel } });
                        setLoading(false);
                    },
                    onError: (error: unknown) => {
                        let apiTitle = "";
                        if (isApiErrorResponse(error)) {
                            const errResponse = error.response;
                            apiTitle =
                                errResponse.data?.title ||
                                errResponse.data?.message ||
                                errResponse.statusText ||
                                translate("SearchByDetails.unknownApiError");
                            setErrors({ submit: apiTitle });
                            console.error("API Error submitting patient:", apiTitle, errResponse);
                        } else if (
                            error &&
                            typeof error === "object" &&
                            "message" in error &&
                            typeof (error as { message?: unknown }).message === "string"
                        ) {
                            setErrors({ submit: (error as { message: string }).message });
                            console.error("Error submitting patient:", (error as { message: string }).message, error);
                        } else {
                            setErrors({ submit: translate("SearchByDetails.unexpectedError") });
                            console.error("Unexpected error submitting patient:", error);
                        }
                        setLoading(false);
                    }
                }
            );
        }
    };

    return (
        <Container fluid>
            <Row className="custom-col-spacing">
                <Col xs={12} md={7} lg={7}>
                    <form className="nhsuk-form-group" autoComplete="off" onSubmit={handleSubmit} >
                        <div style={{ margin: "1rem 0" }}>
                            <label>
                                <input
                                    type="checkbox"
                                    checked={isPowerOfAttorney}
                                    onChange={handleCheckboxChange}
                                    style={{ marginRight: "0.5rem" }}
                                />
                                Requesting an Opt-out on someone else's behalf.
                            </label>
                        </div>
                        <Card cardType="feature">
                            <Card.Content>
                                <Card.Heading>{translate("SearchByDetails.myDetails")}</Card.Heading>

                                <div className={`nhsuk-form-group${errors.surname ? " nhsuk-form-group--error" : ""}`}>
                                    <label className="nhsuk-label" htmlFor="surname">
                                        {translate("SearchByDetails.surnameLabel")}
                                    </label>
                                    <span className="nhsuk-hint" id="surname-hint">
                                        {translate("SearchByDetails.surnameHint")}
                                    </span>
                                    {errors.surname && (
                                        <span className="nhsuk-error-message" id="surname-error">
                                            <strong>{translate("SearchByDetails.errorPrefix")}</strong> {errors.surname}
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
                                        {translate("SearchByDetails.postcodeLabel")}
                                    </label>
                                    <span className="nhsuk-hint" id="postcode-hint">
                                        {translate("SearchByDetails.postcodeHint")}
                                    </span>
                                    {errors.postcode && (
                                        <span className="nhsuk-error-message" id="postcode-error">
                                            <strong>{translate("SearchByDetails.errorPrefix")}</strong> {errors.postcode}
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
                                        {translate("SearchByDetails.dobLegend")}
                                    </legend>
                                    <span className="nhsuk-hint" id="dob-hint">
                                        {translate("SearchByDetails.dobHint")}
                                    </span>
                                    {errors.dob && (
                                        <span className="nhsuk-error-message" id="dob-error">
                                            <strong>{translate("SearchByDetails.errorPrefix")}</strong> {errors.dob}
                                        </span>
                                    )}
                                    <div className="nhsuk-date-input" id="dob" aria-describedby="dob-hint">
                                        <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                                            <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-day">
                                                {translate("SearchByDetails.dobDayLabel")}
                                            </label>
                                            <input
                                                className={`nhsuk-input nhsuk-date-input__input${errors.dob ? " nhsuk-input--error" : ""}`}
                                                id="dob-day"
                                                name="dob-day"
                                                type="text"
                                                inputMode="numeric"
                                                pattern="^(0[1-9]|[12][0-9]|3[01])$"
                                                maxLength={2}
                                                value={dobDay}
                                                onChange={e => handleDayChange(e.target.value)}
                                                style={{ width: "3em" }}
                                                autoComplete="bday-day"
                                            />
                                        </div>
                                        <div className="nhsuk-date-input__item" style={{ display: "inline-block", marginRight: "0.5rem" }}>
                                            <label className="nhsuk-label nhsuk-date-input__label" htmlFor="dob-month">
                                                {translate("SearchByDetails.dobMonthLabel")}
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
                                                {translate("SearchByDetails.dobYearLabel")}
                                            </label>
                                            <input
                                                className={`nhsuk-input nhsuk-date-input__input${errors.dob ? " nhsuk-input--error" : ""}`}
                                                id="dob-year"
                                                name="dob-year"
                                                type="text"
                                                inputMode="numeric"
                                                pattern="^\d{4}$"
                                                maxLength={4}
                                                value={dobYear}
                                                onChange={e => {
                                                    setDobYear(e.target.value.replace(/\D/g, "").slice(0, 4));
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

                        {isPowerOfAttorney && (
                            <Card cardType="feature">
                                <Card.Content>
                                    <Card.Heading>{translate("SearchByDetails.myDetailsRequester")}</Card.Heading>
                                    <Card.Description>
                                        <div style={{ marginBottom: "1.5rem" }}>
                                            <TextInput
                                                label={translate("SearchByDetails.poaFirstnameLabel")}
                                                id="poa-firstname"
                                                name="poa-firstname"
                                                autoComplete="off"
                                                value={poaFirstname}
                                                onChange={handlePoaFirstnameChange}
                                                error={errors.poaFirstname || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            />
                                            <TextInput
                                                label={translate("SearchByDetails.poaSurnameLabel")}
                                                id="poa-surname"
                                                name="poa-surname"
                                                autoComplete="off"
                                                value={poaSurname}
                                                onChange={handlePoaSurnameChange}
                                                error={errors.poaSurname || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            />
                                            <div style={{ marginBottom: "1rem" }}>
                                                <Select
                                                    label={translate("SearchByDetails.poaRelationshipLabel")}
                                                    id="poa-relationship"
                                                    name="poa-relationship"
                                                    aria-label={translate("SearchByDetails.poaRelationshipLabel")}
                                                    aria-required="true"
                                                    required
                                                    value={poaRelationship}
                                                    onChange={handlePoaRelationshipChange}
                                                    error={errors.poaRelationship || undefined}
                                                    style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                                >
                                                    <option value="" disabled>
                                                        {translate("SearchByDetails.poaRelationshipSelect")}
                                                    </option>
                                                    <option value={translate("SearchByDetails.poaRelationshipParent")}>
                                                        {translate("SearchByDetails.poaRelationshipParent")}
                                                    </option>
                                                    <option value={translate("SearchByDetails.poaRelationshipGuardian")}>
                                                        {translate("SearchByDetails.poaRelationshipGuardian")}
                                                    </option>
                                                    <option value={translate("SearchByDetails.poaRelationshipAttorney")}>
                                                        {translate("SearchByDetails.poaRelationshipAttorney")}
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
                                <strong>{translate("SearchByDetails.errorPrefix")}</strong> {errors.submit}
                            </div>
                        )}

                        <button className="nhsuk-button" type="submit" style={{ width: "100%" }} disabled={loading}>
                            {loading ? translate("SearchByDetails.submitting") : translate("SearchByDetails.searchButton")}
                        </button>
                    </form>
                </Col>
                <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                    {isPowerOfAttorney && (
                        <div
                            className="p-4 mb-4"
                            style={{
                                background: "#f4f8fb",
                                border: "1px solid #d1e3f0",
                                borderRadius: "8px",
                                boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                            }}
                        >
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("SearchByDetails.helpGuidanceTitle")}</h2>
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                {translate("SearchByDetails.helpGuidanceSubtitle")}
                            </h3>
                            <p>
                                {translate("SearchByDetails.helpGuidanceText1")}
                            </p>
                            <ul>
                                <li>{translate("SearchByDetails.poaRelationshipParent")}</li>
                                <li>{translate("SearchByDetails.poaRelationshipGuardian")}</li>
                                <li>{translate("SearchByDetails.poaRelationshipAttorney")}</li>
                            </ul>
                            <p>
                                {translate("SearchByDetails.helpGuidanceText2")}
                            </p>
                            <p>
                                {translate("SearchByDetails.helpGuidanceText3")}
                            </p>
                        </div>
                    )}
                </Col>
            </Row>
        </Container >
    );
};

export default SearchByDetails;