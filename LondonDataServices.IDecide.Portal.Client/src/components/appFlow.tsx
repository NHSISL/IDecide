import React, { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { useStep } from "./context/stepContext";
import { ConfirmDetailsPage } from "../pages/confirmDetailsPage";
import { SearchByNhsNumberPage } from "../pages/searchByNhsNumberPage";
import { SendCodePage } from "../pages/sendCodePage";
import { ConfirmCodePage } from "../pages/confirmCodePage";
import { OptInOutPage } from "../pages/optInOutPage";
import { ThankyouPage } from "../pages/thankyouPage";

const stepContent = [
    {
        label: "Provide Your NHS Number",
        content: <SearchByNhsNumberPage />
    },
    {
        label: "Confirm Your Details",
        content: <ConfirmDetailsPage />
    },
    {
        label: "Positive Confirmation"
        // see below for sub step
    },
    {
        label: "Make Your Choice",
        content: <OptInOutPage />
    },
    {
        label: "Receive Notifications",
        content: <ThankyouPage />
    }
];

export const AppFlow = () => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();
    const [confirmationSubStep, setConfirmationSubStep] = useState(0);

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    const nextStep = () => {
        // If on Positive Confirmation, handle sub-step
        if (currentStepIndex === 2) {
            if (confirmationSubStep === 0) {
                setConfirmationSubStep(1);
                return;
            }
        }
        // Reset sub-step when leaving Positive Confirmation
        if (currentStepIndex < stepContent.length - 1) {
            setCurrentStepIndex(currentStepIndex + 1);
            setConfirmationSubStep(0);
        }
    };

    let content;
    if (currentStepIndex === 2) {
        content = confirmationSubStep === 0 ? <SendCodePage /> : <ConfirmCodePage />;
    } else {
        content = stepContent[currentStepIndex].content;
    }

    return (
        <div style={{ padding: "1rem" }}>
            <h2>{stepContent[currentStepIndex].label}</h2>
            <div>{content}</div>
            {currentStepIndex < stepContent.length - 1 && (
                <Button onClick={nextStep} className="mt-3">Next</Button>
            )}
        </div>
    );
};