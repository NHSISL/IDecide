import React, { useState, useEffect, useContext } from "react";
import { useTranslation } from "react-i18next";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";
import { patientViewService } from "../../services/views/patientViewService";
import { TextInput, Button, Select, Card } from "nhsuk-react-components";
import { Container, Row, Col } from "react-bootstrap";
import { StepContext } from "../context/stepContext";
import { PatientLookup } from "../../models/patients/patientLookup";
import { Patient } from "../../models/patients/patient";
import { SearchCriteria } from "../../models/searchCriterias/searchCriteria";
import { useNavigate } from "react-router-dom";

interface ErrorWithResponse {
    response?: {
        status?: number;
        statusText?: string;
        data?: {
            title?: string;
            message?: string;
        };
    };
    message?: string;
}

export const SearchByNhsNumber = ({ powerOfAttourney = false }: {
    powerOfAttourney?: boolean;
}) => {
    const { t: translate } = useTranslation();
    const stepContext = useContext(StepContext);

    useEffect(() => {
        stepContext?.resetStepContext?.();
    }, [stepContext]);

    const [nhsNumberInput, setNhsNumberInput] = useState("");
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
    const [isPowerOfAttorney, setIsPowerOfAttorney] = useState(false);
    const navigate = useNavigate();
    const addPatient = patientViewService.usePostPatientSearch();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, "").slice(0, 10);
        setNhsNumberInput(value);
        if (error) setError("");
    };

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIsPowerOfAttorney(e.target.checked);
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
            setPoaNhsNumberError(translate("SearchByNHSNumber.errorNhsNumber"));
            valid = false;
        }
        if (!poaFirstname.trim()) {
            setPoaFirstnameError(translate("SearchByNHSNumber.errorFirstname"));
            valid = false;
        }
        if (!poaSurname.trim()) {
            setPoaSurnameError(translate("SearchByNHSNumber.errorSurname"));
            valid = false;
        }
        if (!poaRelationship) {
            setPoaRelationshipError(translate("SearchByNHSNumber.errorRelationship"));
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
                setError(translate("SearchByNHSNumber.errorNhsNumber"));
                return;
            }
        }
        setLoading(true);
        const nhsNumberToUse = powerOfAttourney ? poaNhsNumberInput : nhsNumberInput;
        const searchCriteria = new SearchCriteria({ nhsNumber: nhsNumberToUse });
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
                    if (
                        typeof error === "object" &&
                        error !== null &&
                        "response" in error &&
                        typeof (error as ErrorWithResponse).response === "object"
                    ) {
                        const response = (error as ErrorWithResponse).response;
                        if (response?.status === 403) {
                            setError(translate("SearchByNHSNumber.errorNoAccess"));
                        } else {
                            const apiTitle =
                                response?.data?.title ||
                                response?.data?.message ||
                                response?.statusText ||
                                translate("SearchByNHSNumber.unknownApiError");
                            setError(apiTitle);
                        }
                        setLoading(false);
                        return;
                    }
                    if (
                        error &&
                        typeof error === "object" &&
                        "message" in error &&
                        typeof (error as { message?: unknown }).message === "string"
                    ) {
                        setError((error as { message: string }).message);
                        setLoading(false);
                        return;
                    }
                    setError(translate("SearchByNHSNumber.unexpectedError"));
                    setLoading(false);
                }
            }
        );

    };

    return (
        <Container fluid>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
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


                        {!isPowerOfAttorney && (
                            <TextInput
                                label={translate("SearchByNHSNumber.nhsNumberLabel")}
                                hint={translate("SearchByNHSNumber.nhsNumberHint")}
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

                        {isPowerOfAttorney && (
                            <div style={{ marginBottom: "1.5rem" }}>
                                <Card cardType="feature">
                                    <Card.Content>
                                        <Card.Heading style={{ fontSize: "16px" }}>{translate("SearchByNHSNumber.poaNhsNumberLabel")}</Card.Heading>
                                        <TextInput
                                            label={translate("SearchByNHSNumber.nhsNumberLabel")}
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
                                        <Card.Heading style={{ fontSize: "16px" }}>{translate("SearchByNHSNumber.poaMyDetailsHeading")}</Card.Heading>
                                        <TextInput
                                            label={translate("SearchByNHSNumber.poaFirstnameLabel")}
                                            id="poa-firstname"
                                            name="poa-firstname"
                                            autoComplete="off"
                                            value={poaFirstname}
                                            onChange={handlePoaFirstnameChange}
                                            error={poaFirstnameError || undefined}
                                            style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                        />
                                        <TextInput
                                            label={translate("SearchByNHSNumber.poaSurnameLabel")}
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
                                                label={translate("SearchByNHSNumber.poaRelationshipLabel")}
                                                id="poa-relationship"
                                                name="poa-relationship"
                                                aria-label={translate("SearchByNHSNumber.poaRelationshipLabel")}
                                                aria-required="true"
                                                required
                                                value={poaRelationship}
                                                onChange={handlePoaRelationshipChange}
                                                error={poaRelationshipError || undefined}
                                                style={{ maxWidth: "400px", marginBottom: "1rem" }}
                                            >
                                                <option value="" disabled>
                                                    {translate("SearchByNHSNumber.poaRelationshipSelect")}
                                                </option>
                                                <option value={translate("SearchByNHSNumber.poaRelationshipOptions.parent")}>
                                                    {translate("SearchByNHSNumber.poaRelationshipOptions.parent")}
                                                </option>
                                                <option value={translate("SearchByNHSNumber.poaRelationshipOptions.guardian")}>
                                                    {translate("SearchByNHSNumber.poaRelationshipOptions.guardian")}
                                                </option>
                                                <option value={translate("SearchByNHSNumber.poaRelationshipOptions.attorney")}>
                                                    {translate("SearchByNHSNumber.poaRelationshipOptions.attorney")}
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
                                    (powerOfAttourney
                                        ? !poaNhsNumberInput ||
                                        !poaFirstname.trim() ||
                                        !poaSurname.trim() ||
                                        !poaRelationship ||
                                        poaNhsNumberInput.length !== 10
                                        : nhsNumberInput.length !== 10)
                                }
                            >
                                {loading ? translate("SearchByNHSNumber.submittingButton") : translate("SearchByNHSNumber.submitButton")}
                            </Button>
                            <Button
                                type="button"
                                secondary
                                onClick={() => navigate("/patientDetailsSearch")}
                                disabled={loading}
                            >
                                {translate("SearchByNHSNumber.idontknowButton")}
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
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("SearchByNHSNumber.helpGuidanceTitle")}</h2>
                            <h3 className="mb-3" style={{ color: "#005eb8" }}>
                                {translate("SearchByNHSNumber.helpGuidanceNhsNumberHeading")}
                            </h3>
                            <p>{translate("SearchByNHSNumber.helpGuidanceNhsNumberText1")}</p>
                            <p>{translate("SearchByNHSNumber.helpGuidanceNhsNumberText2")}</p>
                            <p>{translate("SearchByNHSNumber.helpGuidanceNhsNumberText3")}</p>
                            <p>{translate("SearchByNHSNumber.helpGuidanceNhsNumberText4")}</p>
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
                            <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("SearchByNHSNumber.helpGuidanceTitle")}</h2>

                            <div style={{ marginBottom: "1.5rem" }}>
                                <h3 style={{ color: "#005eb8" }}>{translate("SearchByNHSNumber.helpGuidanceEligibilityHeading")}</h3>
                                <ul>
                                    <li>{translate("SearchByNHSNumber.helpGuidanceEligibilityList.parent")}</li>
                                    <li>{translate("SearchByNHSNumber.helpGuidanceEligibilityList.guardian")}</li>
                                    <li>{translate("SearchByNHSNumber.helpGuidanceEligibilityList.attorney")}</li>
                                </ul>
                                <p>{translate("SearchByNHSNumber.helpGuidanceEligibilityText")}</p>
                            </div>

                            <div>
                                <h3 style={{ color: "#005eb8" }}>{translate("SearchByNHSNumber.helpGuidancePoaNhsNumberHeading")}</h3>
                                <p>{translate("SearchByNHSNumber.helpGuidancePoaNhsNumberText1")}</p>
                                <p>{translate("SearchByNHSNumber.helpGuidancePoaNhsNumberText2")}</p>
                                <p>{translate("SearchByNHSNumber.helpGuidancePoaNhsNumberText3")}</p>
                            </div>
                        </div>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default SearchByNhsNumber;