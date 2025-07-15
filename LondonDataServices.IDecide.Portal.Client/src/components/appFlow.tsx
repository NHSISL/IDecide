import React, { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { useStep } from "./context/stepContext";

const stepContent = [
    {
        label: "Provide Your NHS Number",
        content: (
            <>
                <input type="text" placeholder="Enter NHS Number" />
            </>
        )
    },
    {
        label: "Confirm Your Details",
        content: <p>Please confirm your name and date of birth.</p>
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
        setCurrentStepIndex(0); // reset if needed
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
