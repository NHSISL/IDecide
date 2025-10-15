import React, { useContext, useEffect, useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttorney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Select, Card } from "nhsuk-react-components";
import { Col, Container, Row, Alert } from "react-bootstrap";
import { StepContext } from "../context/stepContext";
import { useTranslation } from "react-i18next";
import { PatientLookup } from "../../models/patients/patientLookup";
import { SearchCriteria } from "../../models/searchCriterias/searchCriteria";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";

interface SearchByDetailsProps {
    onBack: () => void;
    powerOfAttorney?: boolean;
}

const SearchByDetails: React.FC<SearchByDetailsProps> = ({ onBack, powerOfAttorney }) => {
    const { t: translate } = useTranslation();
    const stepContext = useContext(StepContext);
    const [recaptchaReady, setRecaptchaReady] = useState(false);

    // Standard fields
    const [surname, setSurname] = useState("");
    const [postcode, setPostcode] = useState("");
    const [dobDay, setDobDay] = useState("");
    const [dobMonth, setDobMonth] = useState("");
    const [dobYear, setDobYear] = useState("");
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [error, setError] = useState("");
    const [apiError, setApiError] = useState<string | JSX.Element>("");
    const [loading, setLoading] = useState(false);
    const { configuration } = useFrontendConfiguration();
    const [poaFirstname, setPoaFirstname] = useState("");
    const [poaSurname, setPoaSurname] = useState("");
    const [poaRelationship, setPoaRelationship] = useState("");
    const [poaFirstnameError, setPoaFirstnameError] = useState("");
    const [poaSurnameError, setPoaSurnameError] = useState("");
    const [poaRelationshipError, setPoaRelationshipError] = useState("");
    const addPatient = patientViewService.usePostPatientDetails();

    const handleApiError = useApiErrorHandlerChecks({
        setApiError,
        configuration
    });

    const [recaptchaSiteKey, setRecaptchaSiteKey] = useState<string | undefined>(undefined);
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const { nextStep, setCreatedPatient } = useStep();

    useEffect(() => {
        if (configuration?.recaptchaSiteKey) {
            setRecaptchaSiteKey(configuration.recaptchaSiteKey);
        }
    }, [configuration]);
    useEffect(() => {
        if (stepContext && typeof stepContext.resetStepContext === "function") {
            stepContext.resetStepContext();
        }
    }, [stepContext]);

    useEffect(() => {
        let isMounted = true;
        if (recaptchaSiteKey) {
            loadRecaptchaScript(recaptchaSiteKey)
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
                    if (isMounted) setErrors({ submit: translate("SearchByNHSNumber.errorRecaptchaLoad") });
                });
        }
        return () => { isMounted = false; };
    }, [recaptchaSiteKey, translate]);

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
            return translate("SearchByDetails.dobMonthInvalid");
        }
        if (!/^\d{1,2}$/.test(day) || !/^\d{4}$/.test(year)) {
            return translate("SearchByDetails.dobInvalid");
        }
        const dayNum = parseInt(day, 10);
        const monthNum = parseInt(month, 10);
        const yearNum = parseInt(year, 10);

        if (dayNum < 1 || dayNum > 31) return translate("SearchByDetails.dobDayInvalid");
        if (yearNum < 1900 || yearNum > new Date().getFullYear()) return translate("SearchByDetails.dobYearInvalid");

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
        const filtered = value.replace(/\D/g, "").slice(0, 2);
        setDobMonth(filtered);
        handleFieldChange("dobMonth");
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
        if (powerOfAttorney) {
            if (!poaFirstname.trim()) newErrors.poaFirstname = translate("SearchByDetails.poaFirstnameError");
            if (!poaSurname.trim()) newErrors.poaSurname = translate("SearchByDetails.poaSurnameError");
            if (!poaRelationship) newErrors.poaRelationship = translate("SearchByDetails.poaRelationshipError");
        }

        setErrors(newErrors);

        if (Object.keys(newErrors).length === 0) {
            setLoading(true);
            const dateOfBirth = `${dobYear}-${dobMonth}-${dobDay}`;
            const searchCriteria = new SearchCriteria({
                surname: surname,
                postcode: postcode,
                dateOfBirth: dateOfBirth.toString()
            });
            const patientLookup = new PatientLookup(searchCriteria, []);

            let poaModel = undefined;
            if (powerOfAttorney) {
                poaModel = new PowerOfAttorney({
                    firstName: poaFirstname,
                    surname: poaSurname,
                    relationship: poaRelationship
                });
            }

            if (!recaptchaReady || typeof grecaptcha === "undefined" || !recaptchaSiteKey) {
                setErrors({ submit: translate("SearchByNHSNumber.errorRecaptchaNotReady") });
                setLoading(false);
                return;
            }

            grecaptcha.execute(recaptchaSiteKey, { action: RECAPTCHA_ACTION_SUBMIT }).then((token: string) => {
                addPatient.mutate(
                    patientLookup,
                    {
                        headers: { "X-Recaptcha-Token": token },
                        onSuccess: (createdPatient: Patient) => {
                            setCreatedPatient(createdPatient);
                            nextStep(undefined, undefined, createdPatient, poaModel);
                            setLoading(false);
                        },
                        // eslint-disable-next-line @typescript-eslint/no-explicit-any
                        onError: (error: any) => {
                            const status = error?.response?.status;
                            const errorData = error?.response?.data;
                            const errorTitle = errorData?.title;

                            if (errorTitle === "Patient not found.") {
                                setError(translate("errors.PatientNotFound"));
                                setLoading(false);
                                return;
                            }

                            if (handleApiError(errorTitle)) {
                                setLoading(false);
                                return;
                            }

                            switch (status) {
                                case 400:
                                    setError(translate("errors.400"));
                                    break;
                                case 404:
                                    setError(translate("errors.404"));
                                    break;
                                case 401:
                                    setError(translate("errors.401"));
                                    break;
                                case 500:
                                    setError(
                                        errorTitle === "Patient not found."
                                            ? translate("errors.PatientNotFound")
                                            : translate("errors.500")
                                    );
                                    break;
                                default:
                                    setError(
                                        errorTitle ||
                                        translate("errors.CatchAll")
                                    );
                                    break;
                            }
                            setLoading(false);
                        }
                    }
                );
            }).catch(() => {
                setErrors({ submit: translate("SearchByNHSNumber.errorRecaptchaFailed") });
                setLoading(false);
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
                            aria-label={translate("SearchByDetails.back")}
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
                            {translate("SearchByDetails.back")}
                        </button>

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

                        {powerOfAttorney && (
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
                                                error={poaFirstnameError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            />
                                            <TextInput
                                                label={translate("SearchByDetails.poaSurnameLabel")}
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
                                                    label={translate("SearchByDetails.poaRelationshipLabel")}
                                                    id="poa-relationship"
                                                    name="poa-relationship"
                                                    aria-label={translate("SearchByDetails.poaRelationshipLabel")}
                                                    aria-required="true"
                                                    required
                                                    value={poaRelationship}
                                                    onChange={handlePoaRelationshipChange}
                                                    error={poaRelationshipError || undefined}
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

                        {apiError && (
                            <Alert variant="danger">
                                {apiError}
                            </Alert>
                        )}

                        {error && (
                            <Alert variant="danger">
                                {error}
                            </Alert>
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
                    {powerOfAttorney && (
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