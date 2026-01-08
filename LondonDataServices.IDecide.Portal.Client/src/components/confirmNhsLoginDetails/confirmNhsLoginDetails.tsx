import { useEffect, useState } from "react";
import { useStep } from "../../hooks/useStep";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { Patient } from "../../models/patients/patient";
import { PatientCodeRequest } from "../../models/patients/patientCodeRequest";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";
import { loadRecaptchaScript } from "../../helpers/recaptureLoad";
import { isApiErrorResponse } from "../../helpers/isApiErrorResponse";
import { useApiErrorHandlerChecks } from "../../hooks/useApiErrorHandlerChecks";
import { patientViewService } from "../../services/views/patientViewService";

interface ConfirmNhsLoginDetailsProps { }

export const ConfirmNhsLoginDetails: React.FC<ConfirmNhsLoginDetailsProps> = () => {
    const { t: translate } = useTranslation();
    const { setCurrentStepIndex, nextStep, createdPatient, setCreatedPatient, powerOfAttorney } = useStep();
    const { configuration } = useFrontendConfiguration();
    const RECAPTCHA_SITE_KEY = configuration.recaptchaSiteKey;
    const RECAPTCHA_ACTION_SUBMIT = "submit";
    const updatePatient = patientViewService.useAddPatientNhsLogin();
    const [apiError, setApiError] = useState<string | JSX.Element>("");
    const [info, setInfo] = useState<string | JSX.Element>("");

    const handleApiError = useApiErrorHandlerChecks({
        setApiError,
        configuration
    });

    useEffect(() => {
        fetch('/patientinfo')
            .then(d => d.json())
            .then(r => {
                setCreatedPatient(new Patient({
                    nhsNumber: r.nhs_number,
                    givenName: r.given_name,
                    surname: r.family_name,
                    dateOfBirth: r.birthdate ? new Date(r.birthdate) : undefined,
                    email: r.email,
                    phone: r.phone_number
                }));
            });
    }, [setCreatedPatient]);

    const handleNoClick = () => {
        setCurrentStepIndex(0);
    };

    const handleSubmit = async () => {
        setApiError("");
        setInfo("");

        const phoneNumber = createdPatient!.phone;

        try {
            await loadRecaptchaScript(RECAPTCHA_SITE_KEY);

            if (!(window as any).grecaptcha || !(window as any).grecaptcha.execute) {
                setApiError("reCAPTCHA failed to load. Please try again later.");
                return;
            }

            const token = await (window as any).grecaptcha.execute(
                RECAPTCHA_SITE_KEY,
                { action: RECAPTCHA_ACTION_SUBMIT }
            );

            updatePatient.mutate(
                phoneNumber!,
                {
                    headers: { "X-Recaptcha-Token": token },
                    onSuccess: () => {
                        setApiError("");
                        setInfo("");
                        nextStep(undefined, undefined, createdPatient!, powerOfAttorney!);
                    },
                    onError: (error: unknown) => {
                        if (isApiErrorResponse(error)) {
                            const errorResponse = error?.response;

                            const errorTitle =
                                errorResponse?.data?.title ||
                                errorResponse?.data?.message ||
                                errorResponse.statusText ||
                                "Unknown API error";

                            if (!handleApiError(errorTitle)) {
                                setApiError(errorTitle);
                            }
                            setApiError(errorTitle);
                        } else if (
                            error &&
                            typeof error === "object" &&
                            "message" in error &&
                            typeof (error as { message?: unknown }).message === "string"
                        ) {
                            setApiError((error as { message: string }).message);
                            console.error("Error updating patient:", (error as { message: string }).message, error);
                        } else {
                            setApiError("An unexpected error occurred.");
                        }
                    }
                }
            );
        } catch (err) {
            setApiError("Error executing reCAPTCHA.");
            console.error("Error executing reCAPTCHA:", err);
        }
    };

    const handleYesClick = () => {

        //NEED TO SAVE PATIENT
        handleSubmit()

        //nextStep(undefined, undefined, createdPatient!, powerOfAttorney!);
    };

    if (!createdPatient) {
        return <div>{translate("ConfirmDetails.noPatientDetails")}</div>;
    }

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={6} lg={6}>
                <div>
                    <h4 style={{ fontWeight: 700, fontSize: "1.5rem", margin: "0 0 1rem 0", color: "#212529" }}>{translate("ConfirmDetails.isThisYou")}</h4>
                    <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.name")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.givenName + ',' + createdPatient.surname}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.email")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.email}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.mobileNumber")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.phone}</dd>
                        </div>
                    </dl>
                    {/*<Button*/}
                    {/*    onClick={() => {*/}
                    {/*        fetch('/logout', { method: 'POST' }).then(d => {*/}
                    {/*            if (d.ok) {*/}
                    {/*                window.location.href = '/';*/}
                    {/*            }*/}
                    {/*        });*/}
                    {/*    }}*/}
                    {/*>*/}
                    {/*    Logout*/}
                    {/*</Button>*/}
                    <div style={{ display: "flex", gap: "1rem", marginBottom: "0.2rem" }}>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1 }}
                            onClick={handleYesClick}
                        >
                            {translate("ConfirmDetails.yes")}
                        </button>
                        <button
                            type="button"
                            className="nhsuk-button nhsuk-button--secondary"
                            style={{ flex: 1 }}
                            onClick={handleNoClick}
                        >
                            {translate("ConfirmDetails.no")}
                        </button>
                    </div>
                </div>

                {apiError && (
                    <Alert variant="danger">
                        <div id="code-error">{apiError}</div>
                    </Alert>
                )}

                {info && (
                    <Alert variant="info">
                        <div id="code-info">{info}</div>
                    </Alert>
                )}
            </Col>
            <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                <div
                    className="p-4 mb-4"
                    style={{
                        background: "#f4f8fb",
                        border: "1px solid #d1e3f0",
                        borderRadius: "8px",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                    }}
                >
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>
                        {translate("ConfirmDetails.helpGuidance")}
                    </h2>
                    <h3 className="mb-3" style={{ color: "#005eb8" }}>
                        {translate("ConfirmDetails.checkingYourDetails")}
                    </h3>
                    <p>
                        {translate("ConfirmDetails.reviewDetails")}
                    </p>
                    <p>
                        {translate("ConfirmDetails.detailsCorrect")}
                    </p>
                    <p>
                        {translate("ConfirmDetails.detailsIncorrect")}
                    </p>
                    <p>
                        {translate("ConfirmDetails.alreadyReceivedCode")}
                    </p>
                    <p>
                        {translate("ConfirmDetails.securityInfo")}
                    </p>
                </div>
            </Col>
        </Row>
    );
};

export default ConfirmNhsLoginDetails;