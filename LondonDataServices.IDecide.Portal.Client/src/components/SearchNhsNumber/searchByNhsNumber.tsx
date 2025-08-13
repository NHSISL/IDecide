import React, { useState, useEffect, useContext } from "react";
import { useTranslation } from "react-i18next";
import { useStep } from "../../hooks/useStep";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button, Select, Card } from "nhsuk-react-components";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { Container, Row, Col } from "react-bootstrap";
import { StepContext } from "../context/stepContext";

const RECAPTCHA_SITE_KEY = "6LcOJn4rAAAAAIUdB70R9BqkfPFD-bPYTk6ojRGg";

export const SearchByNhsNumber = ({ onIDontKnow, powerOfAttourney = false }: {
    onIDontKnow: (powerOfAttourney: boolean) => void;
    powerOfAttourney?: boolean;
}) => {
    const { t } = useTranslation();
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
    const { nextStep, setCreatedPatient } = useStep();
    const addPatient = patientViewService.usePostPatientNhsNumber();

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
                if (isMounted) setError(t("SearchBySHSNumber.errorRecaptchaLoad"));
            });
        return () => { isMounted = false; };
    }, [t]);

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
            setPoaNhsNumberError(t("SearchBySHSNumber.errorNhsNumber"));
            valid = false;
        }
        if (!poaFirstname.trim()) {
            setPoaFirstnameError(t("SearchBySHSNumber.errorFirstname"));
            valid = false;
        }
        if (!poaSurname.trim()) {
            setPoaSurnameError(t("SearchBySHSNumber.errorSurname"));
            valid = false;
        }
        if (!poaRelationship) {
            setPoaRelationshipError(t("SearchBySHSNumber.errorRelationship"));
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
                setError(t("SearchBySHSNumber.errorNhsNumber"));
                return;
            }
        }

        if (!recaptchaReady || typeof grecaptcha === "undefined") {
            setError(t("SearchBySHSNumber.errorRecaptchaNotReady"));
            return;
        }
        setLoading(true);
        try {
            grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: "submit" }).then((token: string) => {
                const nhsNumberToUse = powerOfAttourney ? poaNhsNumberInput : nhsNumberInput;
                const patientToCreate = new Patient({ id: "", nhsNumber: nhsNumberToUse, recaptchaToken: token });

                let poaModel = undefined;
                if (powerOfAttourney) {
                    poaModel = new PowerOfAttourney({
                        firstName: poaFirstname,
                        surname: poaSurname,
                        relationship: poaRelationship
                    });
                }

                addPatient.mutate(patientToCreate, {
                    onSuccess: (createdPatient) => {
                        setCreatedPatient(createdPatient);
                        nextStep(undefined, nhsNumberToUse, createdPatient, poaModel);
                        setLoading(false);
                    },
                    onError: () => {
                        setError(t("SearchBySHSNumber.errorCreatePatient"));
                        setLoading(false);
                    }
                });
            });
        } catch {
            setError(t("SearchBySHSNumber.errorRecaptchaFailed"));
            setLoading(false);
        }
    };

    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
                    <form autoComplete="off" onSubmit={handleSubmit}>
                        {!powerOfAttourney && (
                            <TextInput
                                label={t("SearchBySHSNumber.nhsNumberLabel")}
                                hint={t("SearchBySHSNumber.nhsNumberHint")}
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
                                <Card cardType="feature">
                                    <Card.Content>
                                        <Card.Heading>{t("SearchBySHSNumber.poaNhsNumberLabel")}</Card.Heading>
                                        <TextInput
                                            label={t("SearchBySHSNumber.nhsNumberLabel")}
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
                                        <Card.Heading>{t("SearchBySHSNumber.poaMyDetailsHeading")}</Card.Heading>
                                        <TextInput
                                            label={t("SearchBySHSNumber.poaFirstnameLabel")}
                                            id="poa-firstname"
                                            name="poa-firstname"
                                            autoComplete="off"
                                            value={poaFirstname}
                                            onChange={handlePoaFirstnameChange}
                                            error={poaFirstnameError || undefined}
                                            style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                        />
                                        <TextInput
                                            label={t("SearchBySHSNumber.poaSurnameLabel")}
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
                                                label={t("SearchBySHSNumber.poaRelationshipLabel")}
                                                id="poa-relationship"
                                                name="poa-relationship"
                                                aria-label={t("SearchBySHSNumber.poaRelationshipLabel")}
                                                aria-required="true"
                                                required
                                                value={poaRelationship}
                                                onChange={handlePoaRelationshipChange}
                                                error={poaRelationshipError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            >
                                                <option value="" disabled>
                                                    {t("SearchBySHSNumber.poaRelationshipSelect")}
                                                </option>
                                                <option value={t("SearchBySHSNumber.poaRelationshipOptions.parent")}>
                                                    {t("SearchBySHSNumber.poaRelationshipOptions.parent")}
                                                </option>
                                                <option value={t("SearchBySHSNumber.poaRelationshipOptions.guardian")}>
                                                    {t("SearchBySHSNumber.poaRelationshipOptions.guardian")}
                                                </option>
                                                <option value={t("SearchBySHSNumber.poaRelationshipOptions.attorney")}>
                                                    {t("SearchBySHSNumber.poaRelationshipOptions.attorney")}
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
                                    (powerOfAttourney
                                        ? !poaNhsNumberInput ||
                                        !poaFirstname.trim() ||
                                        !poaSurname.trim() ||
                                        !poaRelationship ||
                                        poaNhsNumberInput.length !== 10
                                        : nhsNumberInput.length !== 10)
                                }
                            >
                                {loading ? t("SearchBySHSNumber.submittingButton") : t("SearchBySHSNumber.submitButton")}
                            </Button>
                            <Button
                                type="button"
                                secondary
                                onClick={() => onIDontKnow(powerOfAttourney)}
                                disabled={loading}
                            >
                                {t("SearchBySHSNumber.idontknowButton")}
                            </Button>
                        </div>
                    </form>
                </Col>
                <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                    {!powerOfAttourney && (
                        <div
                            className="p-4 mb-4"
                            style={{
                                background: "#f4f8fb",
                                border: "1px solid #d1e3f0",
                                borderRadius: "8px",
                                boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                            }}
                        >
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{t("SearchBySHSNumber.helpGuidanceTitle")}</h2>
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                {t("SearchBySHSNumber.helpGuidanceNhsNumberHeading")}
                            </h3>
                            <p>{t("SearchBySHSNumber.helpGuidanceNhsNumberText1")}</p>
                            <p>{t("SearchBySHSNumber.helpGuidanceNhsNumberText2")}</p>
                            <p>{t("SearchBySHSNumber.helpGuidanceNhsNumberText3")}</p>
                            <p>{t("SearchBySHSNumber.helpGuidanceNhsNumberText4")}</p>
                        </div>
                    )}
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
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{t("SearchBySHSNumber.helpGuidanceTitle")}</h2>

                            <div style={{ marginBottom: "1.5rem" }}>
                                <h3 style={{ color: "#005eb8" }}>{t("SearchBySHSNumber.helpGuidanceEligibilityHeading")}</h3>
                                <ul>
                                    <li>{t("SearchBySHSNumber.helpGuidanceEligibilityList.parent")}</li>
                                    <li>{t("SearchBySHSNumber.helpGuidanceEligibilityList.guardian")}</li>
                                    <li>{t("SearchBySHSNumber.helpGuidanceEligibilityList.attorney")}</li>
                                </ul>
                                <p>{t("SearchBySHSNumber.helpGuidanceEligibilityText")}</p>
                            </div>

                            <div>
                                <h3 style={{ color: "#005eb8" }}>{t("SearchBySHSNumber.helpGuidancePoaNhsNumberHeading")}</h3>
                                <p>{t("SearchBySHSNumber.helpGuidancePoaNhsNumberText1")}</p>
                                <p>{t("SearchBySHSNumber.helpGuidancePoaNhsNumberText2")}</p>
                                <p>{t("SearchBySHSNumber.helpGuidancePoaNhsNumberText3")}</p>
                            </div>
                        </div>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default SearchByNhsNumber;