import React from "react";
import { useStep } from "../context/stepContext";
import { patientViewService } from "../../services/views/patientViewService";
import { Patient } from "../../models/patients/patient";

interface PositiveConfirmationProps {
    goToConfirmCode: () => void;
}

const PositiveConfirmation: React.FC<PositiveConfirmationProps> = ({ goToConfirmCode }) => {
    const { createdPatient } = useStep();
    const updatePatient = patientViewService.useUpdatePatient();
    const patientToUpdate = new Patient(createdPatient);

    const handleSubmit = (method: "email" | "sms" | "letter") => {
        console.log(createdPatient.nhsNumber + ',' + method);
        patientToUpdate.codeNotificationDecision = method;
        updatePatient.mutate(patientToUpdate, {
            onSuccess: (createdPatient) => {
                console.log("Updated patient:", createdPatient);

                goToConfirmCode();
            },
            onError: (error: any) => {
                console.error("Error updating patient:", error);
            }
        });
    };

    if (!createdPatient) {
        return <div>No patient details available.</div>;
    }

    return (
        <div className="mt-4">
            <h2>Confirmation required</h2>
            <p>Please confirm these details are correct before continuing:</p>
            <dl className="nhsuk-summary-list" style={{ marginBottom: "2rem" }}>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Name</dt>
                    <dd className="nhsuk-summary-list__value">{createdPatient.surname}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Email</dt>
                    <dd className="nhsuk-summary-list__value">{createdPatient.emailAddress}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Mobile Number</dt>
                    <dd className="nhsuk-summary-list__value">{createdPatient.mobileNumber}</dd>
                </div>
                <div className="nhsuk-summary-list__row">
                    <dt className="nhsuk-summary-list__key">Address</dt>
                    <dd className="nhsuk-summary-list__value">{createdPatient.address}</dd>
                </div>
            </dl>

            <p style={{ fontWeight: 500, marginBottom: "1rem" }}>
                We need to send a code to you, how would you like to receive it:
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
                    onClick={() => handleSubmit("email")}
                >
                    Email
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={() => handleSubmit("sms")}
                >
                    SMS
                </button>
                <button
                    type="button"
                    className="nhsuk-button"
                    style={{ flex: 1, minWidth: 120 }}
                    onClick={() => handleSubmit("letter")}
                >
                    Letter
                </button>
            </div>
        </div>
    );
};

export default PositiveConfirmation;