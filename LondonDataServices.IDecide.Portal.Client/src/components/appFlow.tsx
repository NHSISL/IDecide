import React, { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { useStep } from "./context/stepContext";
import { ConfirmDetailsPage } from "../pages/confirmDetailsPage";
import { ConfirmNhsNumber } from "../pages/confirmNhsNumberPage";

const stepContent = [
    {
        label: "Provide Your NHS Number",
        content: <ConfirmNhsNumber></ConfirmNhsNumber>
    },
    {
        label: "Confirm Your Details",
        content: <ConfirmDetailsPage></ConfirmDetailsPage>
    },
    {
        label: "Positive Confirmation",
        content: <p>You've reviewed and validated your information.</p>
    },
    {
        label: "Make Your Choice",
        content: <p>Select your opt-out preference.</p>
    },
    {
        label: "Receive Notifications",
        content: <p>Choose how to be notified.</p>
    }
];

export const AppFlow = () => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    const nextStep = () => {
        if (currentStepIndex < stepContent.length - 1) {
            setCurrentStepIndex(currentStepIndex + 1);
        }
    };

    return (
        <div style={{ padding: "1rem" }}>
            <h2>{stepContent[currentStepIndex].label}</h2>
            <div>{stepContent[currentStepIndex].content}</div>
            {currentStepIndex < stepContent.length - 1 && (
                <Button onClick={nextStep} className="mt-3">Next</Button>
            )}
        </div>
    );
};
