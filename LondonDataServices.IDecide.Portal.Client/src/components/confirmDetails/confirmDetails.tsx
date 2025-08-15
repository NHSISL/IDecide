import React, { useEffect } from "react";
import { useStep } from "../../hooks/useStep";
import { Row, Col, Alert } from "react-bootstrap";
import { useTranslation } from "react-i18next";

interface ConfirmDetailsProps {
    goToConfirmCode: () => void;
}

export const ConfirmDetails: React.FC<ConfirmDetailsProps> = ({ goToConfirmCode }) => {
    const { t: translate } = useTranslation();
    const { setCurrentStepIndex, nextStep, createdPatient, powerOfAttourney } = useStep();

    useEffect(() => {
    }, [powerOfAttourney]);

    const handleNoClick = () => {
        setCurrentStepIndex(0);
    };

    const handleYesClick = () => {
        nextStep(undefined, undefined, createdPatient!, powerOfAttourney!);
    };

    if (!createdPatient) {
        return <div>{translate("ConfirmDetails.noPatientDetails")}</div>;
    }

    return (
        <Row className="custom-col-spacing">
            <Col xs={12} md={6} lg={6}>
                <div className="mt-4">
                    {powerOfAttourney && (
                        <Alert variant="info" className="d-flex align-items-center" style={{ marginBottom: "0.75rem", padding: "0.75rem" }}>
                            <div className="me-2" style={{ fontSize: "1.5rem", color: "#6c757d" }}>
                            </div>
                            <div>
                                <div style={{ fontSize: "1rem", marginBottom: "0.25rem", color: "#6c757d", fontWeight: 500 }}>
                                    {translate("ConfirmDetails.powerOfAttorneyDetails")}
                                </div>
                                <dl className="mb-0" style={{ fontSize: "0.95rem", color: "#6c757d" }}>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmDetails.powerOfAttorneyName")}:</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.firstName} {powerOfAttourney.surname}</strong>
                                        </dd>
                                    </div>
                                    <div>
                                        <dt style={{ display: "inline", fontWeight: 500 }}>{translate("ConfirmDetails.powerOfAttorneyRelationship")}:</dt>
                                        <dd style={{ display: "inline", marginLeft: "0.5rem" }}>
                                            <strong>{powerOfAttourney.relationship}</strong>
                                        </dd>
                                    </div>
                                </dl>
                            </div>
                        </Alert>
                    )}
                    <h4 style={{ fontWeight: 700, fontSize: "1.5rem", margin: "1.5rem 0 1rem 0", color: "#212529" }}>{translate("ConfirmDetails.isThisYou")}</h4>
                    <form className="nhsuk-form-group">
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
                                <dd className="nhsuk-summary-list__value">07******084</dd>
                            </div>
                            <div className="nhsuk-summary-list__row">
                                <dt className="nhsuk-summary-list__key">{translate("ConfirmDetails.address")}</dt>
                                <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                            </div>
                        </dl>

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

                        <button
                            type="button"
                            className="nhsuk-button"
                            style={{ width: "100%", marginBottom: 0, borderBottom: "none" }}
                            onClick={goToConfirmCode}
                        >
                            {translate("ConfirmDetails.useExistingCode")}
                        </button>
                    </form>
                </div>
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
                    <h2 className="mb-3" style={{ color: "#005eb8" }}>{translate("ConfirmDetails.helpGuidance")}</h2>
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

export default ConfirmDetails;