import React, { useState, useEffect, useContext } from "react";
import { useTranslation } from "react-i18next";
import { useStep } from "../../hooks/useStep";
import { PowerOfAttorney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button, Select, Card } from "nhsuk-react-components";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { Container, Row, Col } from "react-bootstrap";
import { StepContext } from "../context/stepContext";
import { PatientLookup } from "../../models/patients/patientLookup";
import { Patient } from "../../models/patients/patient";
import { SearchCriteria } from "../../models/searchCriterias/searchCriteria";

export const SearchByNhsNumber = ({ onIDontKnow, powerOfAttorney = false }: {
    onIDontKnow: (powerOfAttorney: boolean) => void;
    powerOfAttorney?: boolean;
}) => {
    const { t: translate } = useTranslation();
    const stepContext = useContext(StepContext);

    useEffect(() => {
        stepContext?.resetStepContext?.();
    }, [stepContext]);

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

    const [recaptchaSiteKey, setRecaptchaSiteKey] = useState<string | undefined>(undefined);
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const { nextStep, setCreatedPatient } = useStep();

    const addPatient = patientViewService.usePostPatientSearch();

    useEffect(() => {
        if (configuration?.recaptchaSiteKey) {
            setRecaptchaSiteKey(configuration.recaptchaSiteKey);
        }
    }, [configuration]);

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
                    if (isMounted) setError(translate("SearchBySHSNumber.errorRecaptchaLoad"));
                });
        }
        return () => { isMounted = false; };
    }, [recaptchaSiteKey, translate]);

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
            setPoaNhsNumberError(translate("SearchBySHSNumber.errorNhsNumber"));
            valid = false;
        }
        if (!poaFirstname.trim()) {
            setPoaFirstnameError(translate("SearchBySHSNumber.errorFirstname"));
            valid = false;
        }
        if (!poaSurname.trim()) {
            setPoaSurnameError(translate("SearchBySHSNumber.errorSurname"));
            valid = false;
        }
        if (!poaRelationship) {
            setPoaRelationshipError(translate("SearchBySHSNumber.errorRelationship"));
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

        if (powerOfAttorney) {
            if (!validatePoaFields()) return;
        } else {
            if (nhsNumberInput.length !== 10) {
                setError(translate("SearchBySHSNumber.errorNhsNumber"));
                return;
            }
        }

        if (!recaptchaReady || typeof grecaptcha === "undefined" || !recaptchaSiteKey) {
            setError(translate("SearchBySHSNumber.errorRecaptchaNotReady"));
            return;
        }
        setLoading(true);
        try {
            grecaptcha.execute(recaptchaSiteKey, { action: RECAPTCHA_ACTION_SUBMIT }).then((token: string) => {
                const nhsNumberToUse = powerOfAttorney ? poaNhsNumberInput : nhsNumberInput;
                const searchCriteria = new SearchCriteria({ nhsNumber: nhsNumberToUse });
                const patientLookup = new PatientLookup(searchCriteria, []);

                let poaModel = undefined;
                if (powerOfAttorney) {
                    poaModel = new PowerOfAttorney({
                        firstName: poaFirstname,
                        surname: poaSurname,
                        relationship: poaRelationship
                    });
                }

                addPatient.mutate(
                    patientLookup,
                    {
                        headers: { "X-Recaptcha-Token": token },
                        onSuccess: (createdPatient: Patient) => {
                            setCreatedPatient(createdPatient);
                            nextStep(undefined, nhsNumberToUse, createdPatient, poaModel);
                            setLoading(false);
                        },
                        onError: () => {
                            setError(translate("SearchBySHSNumber.errorCreatePatient"));
                            setLoading(false);
                        }
                    }
                );
            });
        } catch {
            setError(translate("SearchBySHSNumber.errorRecaptchaFailed"));
            setLoading(false);
        }
    };

    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
                    <form autoComplete="off" onSubmit={handleSubmit}>
                        {!powerOfAttorney && (
                            <TextInput
                                label={translate("SearchBySHSNumber.nhsNumberLabel")}
                                hint={translate("SearchBySHSNumber.nhsNumberHint")}
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

                        {powerOfAttorney && (
                            <div style={{ marginBottom: "1.5rem" }}>
                                <Card cardType="feature">
                                    <Card.Content>
                                        <Card.Heading>{translate("SearchBySHSNumber.poaNhsNumberLabel")}</Card.Heading>
                                        <TextInput
                                            label={translate("SearchBySHSNumber.nhsNumberLabel")}
                                            id="poa-nhs-number"
                                            name="poa-nhs-number"
                                            inputMode="numeric"
                                            pattern="\d*"
                                            maxLength={10}
                                            autoComplete="off"
                                            value={poaNhsNumberInput}
                                            onChange={handlePoaNhsNumberChange}
                                            error={poaNhsNumberError || undefined}
                                            style={{ maxWidth: "300px", marginBottom: "1rem" }}
                                        />
                                    </Card.Content>
                                </Card>

                                <Card cardType="feature">
                                    <Card.Content>
                                        <Card.Heading>{translate("SearchBySHSNumber.poaMyDetailsHeading")}</Card.Heading>
                                        <TextInput
                                            label={translate("SearchBySHSNumber.poaFirstnameLabel")}
                                            id="poa-firstname"
                                            name="poa-firstname"
                                            autoComplete="off"
                                            value={poaFirstname}
                                            onChange={handlePoaFirstnameChange}
                                            error={poaFirstnameError || undefined}
                                            style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                        />
                                        <TextInput
                                            label={translate("SearchBySHSNumber.poaSurnameLabel")}
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
                                                label={translate("SearchBySHSNumber.poaRelationshipLabel")}
                                                id="poa-relationship"
                                                name="poa-relationship"
                                                aria-label={translate("SearchBySHSNumber.poaRelationshipLabel")}
                                                aria-required="true"
                                                required
                                                value={poaRelationship}
                                                onChange={handlePoaRelationshipChange}
                                                error={poaRelationshipError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            >
                                                <option value="" disabled>
                                                    {translate("SearchBySHSNumber.poaRelationshipSelect")}
                                                </option>
                                                <option value={translate("SearchBySHSNumber.poaRelationshipOptions.parent")}>
                                                    {translate("SearchBySHSNumber.poaRelationshipOptions.parent")}
                                                </option>
                                                <option value={translate("SearchBySHSNumber.poaRelationshipOptions.guardian")}>
                                                    {translate("SearchBySHSNumber.poaRelationshipOptions.guardian")}
                                                </option>
                                                <option value={translate("SearchBySHSNumber.poaRelationshipOptions.attorney")}>
                                                    {translate("SearchBySHSNumber.poaRelationshipOptions.attorney")}
                                                </option>
                                            </Select>
                                        </div>
                                    </Card.Content>
                                </Card>
                            </div>
                        )}
                        <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem", marginTop: "1rem" }}>
                            <Button
                                type="submit"
                                disabled={
                                    loading ||
                                    !recaptchaReady ||
                                    (powerOfAttorney
                                        ? !poaNhsNumberInput ||
                                        !poaFirstname.trim() ||
                                        !poaSurname.trim() ||
                                        !poaRelationship ||
                                        poaNhsNumberInput.length !== 10
                                        : nhsNumberInput.length !== 10)
                                }
                            >
                                {loading ? translate("SearchBySHSNumber.submittingButton") : translate("SearchBySHSNumber.submitButton")}
                            </Button>
                            <Button
                                type="button"
                                secondary
                                onClick={() => onIDontKnow(powerOfAttorney)}
                                disabled={loading}
                            >
                                {translate("SearchBySHSNumber.idontknowButton")}
                            </Button>
                        </div>
                    </form>
                </Col>
                <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                    {!powerOfAttorney && (
                        <div
                            className="p-4 mb-4"
                            style={{
                                background: "#f4f8fb",
                                border: "1px solid #d1e3f0",
                                borderRadius: "8px",
                                boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                            }}
                        >
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("SearchBySHSNumber.helpGuidanceTitle")}</h2>
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                {translate("SearchBySHSNumber.helpGuidanceNhsNumberHeading")}
                            </h3>
                            <p>{translate("SearchBySHSNumber.helpGuidanceNhsNumberText1")}</p>
                            <p>{translate("SearchBySHSNumber.helpGuidanceNhsNumberText2")}</p>
                            <p>{translate("SearchBySHSNumber.helpGuidanceNhsNumberText3")}</p>
                            <p>{translate("SearchBySHSNumber.helpGuidanceNhsNumberText4")}</p>
                        </div>
                    )}
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
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("SearchBySHSNumber.helpGuidanceTitle")}</h2>

                            <div style={{ marginBottom: "1.5rem" }}>
                                <h3 style={{ color: "#005eb8" }}>{translate("SearchBySHSNumber.helpGuidanceEligibilityHeading")}</h3>
                                <ul>
                                    <li>{translate("SearchBySHSNumber.helpGuidanceEligibilityList.parent")}</li>
                                    <li>{translate("SearchBySHSNumber.helpGuidanceEligibilityList.guardian")}</li>
                                    <li>{translate("SearchBySHSNumber.helpGuidanceEligibilityList.attorney")}</li>
                                </ul>
                                <p>{translate("SearchBySHSNumber.helpGuidanceEligibilityText")}</p>
                            </div>

                            <div>
                                <h3 style={{ color: "#005eb8" }}>{translate("SearchBySHSNumber.helpGuidancePoaNhsNumberHeading")}</h3>
                                <p>{translate("SearchBySHSNumber.helpGuidancePoaNhsNumberText1")}</p>
                                <p>{translate("SearchBySHSNumber.helpGuidancePoaNhsNumberText2")}</p>
                                <p>{translate("SearchBySHSNumber.helpGuidancePoaNhsNumberText3")}</p>
                            </div>
                        </div>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default SearchByNhsNumber;