import React from "react";
import { useStep } from "../../hooks/useStep";
import { patientViewService } from "../../services/views/patientViewService";
import { GenerateCodeRequest } from "../../models/patients/generateCodeRequest";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from "react-i18next";

interface PositiveConfirmationProps {
    goToConfirmCode: (createdPatient: GenerateCodeRequest) => void;
}

const PositiveConfirmation: React.FC<PositiveConfirmationProps> = ({ goToConfirmCode }) => {
    const { t: translate } = useTranslation();
    const { createdPatient, powerOfAttourney } = useStep();
    const updatePatient = patientViewService.useUpdatePatient();

    if (!createdPatient) {
        return <div>{translate("PositiveConfirmation.noPatientDetails")}</div>;
    }

    const patientToUpdate = new GenerateCodeRequest(createdPatient);

    const handleSubmit = (method: "Email" | "SMS" | "Letter") => {
        patientToUpdate.notificationPreference = method;

        if (powerOfAttourney) {
            patientToUpdate.poaFirstName = powerOfAttourney.firstName;
            patientToUpdate.poaSurname = powerOfAttourney.surname;
            patientToUpdate.poaRelationship = powerOfAttourney.relationship;
        }

        updatePatient.mutate(patientToUpdate, {
            onSuccess: () => {
                goToConfirmCode(patientToUpdate);
            },
            onError: (error: unknown) => {
                if (error instanceof Error) {
                    console.error("Error updating patient:", error.message);
                } else {
                    console.error("Error updating patient:", error);
                }
            }
        });
    };

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={7} lg={7}>
                <div className="mt-4">

                    {powerOfAttourney && (
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    {translate("PositiveConfirmation.poaDetailsTitle")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("PositiveConfirmation.poaNameLabel")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("PositiveConfirmation.poaRelationshipLabel")}</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}

                    <h2>{translate("PositiveConfirmation.confirmationRequiredTitle")}</h2>
                    <p>{translate("PositiveConfirmation.confirmationRequiredDescription")}</p>
                    <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryName")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.surname}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryEmail")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.emailAddress}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryMobile")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.phoneNumber}</dd>
                        </div>
                        <div className="nhsuk-summary-list__row">
                            <dt className="nhsuk-summary-list__key">{translate("PositiveConfirmation.summaryAddress")}</dt>
                            <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                        </div>
                    </dl>

                    <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                        {translate("PositiveConfirmation.chooseMethod")}
                    </p>
                    <div style={{
                        display: "flex",
                        gap: "1rem",
                        marginBottom: "2rem",
                        flexWrap: "wrap"
                    }}>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("Email")}
                            disabled={!createdPatient.emailAddress}
                        >
                            {translate("PositiveConfirmation.methodEmail")}
                        </button>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("SMS")}
                            disabled={!createdPatient.phoneNumber}
                        >
                            {translate("PositiveConfirmation.methodSMS")}
                        </button>
                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ flex: 1, minWidth: 120 }}
                            onClick={() => handleSubmit("Letter")}
                            disabled={!createdPatient.address}
                        >
                            {translate("PositiveConfirmation.methodLetter")}
                        </button>
                    </div>
                </div>
            </Col>
            <Col xs={12} md={5} lg={5} className="custom-col-spacing">
                <div
                    className="p-4 mb-4"
                    style={{
                        background: "#f4f8fb",
                        border: "1px solid #d1e3f0",
                        borderRadius: "8px",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.04)",
                    }}
                >
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("PositiveConfirmation.helpGuidanceTitle")}</h2>
                    <h3 className="mb-3" style={{ color: "#005eb8" }}>
                        {translate("PositiveConfirmation.helpReceivingCodeTitle")}
                    </h3>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription1")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription2")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription3")}
                    </p>
                    <p>
                        {translate("PositiveConfirmation.helpReceivingCodeDescription4")}
                    </p>
                </div>
            </Col>
        </Row>
    );
};

export default PositiveConfirmation;