import React, { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { useStep } from "./context/stepContext";
import { ConfirmDetailsPage } from "../pages/confirmDetailsPage";
import { SearchByNhsNumberPage } from "../pages/searchByNhsNumberPage";
import { SearchByDetailsPage } from "../pages/searchByDetailsPage";
import { ConfirmCodePage } from "../pages/confirmCodePage";
import { OptInOutPage } from "../pages/OptInOutPage";
import { ThankyouPage } from "../pages/thankyouPage";
import PositiveConfirmation from "./positiveConfirmation/positiveConfirmation";

const stepContent = [
    { label: "Provide Your NHS Number",},
    { label: "Confirm Your Details",},
    { label: "Positive Confirmation"},
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
    const [nhsNumberSubStep, setNhsNumberSubStep] = useState(0); // 0: NHS Number, 1: Search by Details
    const [confirmationSubStep, setConfirmationSubStep] = useState(0);

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    const nextStep = () => {
        // NHS Number sub-step logic
        if (currentStepIndex === 0 && nhsNumberSubStep === 1) {
            setCurrentStepIndex(currentStepIndex + 1);
            setNhsNumberSubStep(0);
            return;
        }
        // Positive Confirmation sub-step logic
        if (currentStepIndex === 2) {
            if (confirmationSubStep === 0) {
                setConfirmationSubStep(1);
                return;
            }
        }
        // Reset sub-steps when leaving their main step
        if (currentStepIndex < stepContent.length - 1) {
            setCurrentStepIndex(currentStepIndex + 1);
            setConfirmationSubStep(0);
            setNhsNumberSubStep(0);
        }
    };

    let content;
    if (currentStepIndex === 0) {
        content =
            nhsNumberSubStep === 0 ? (
                <SearchByNhsNumberPage onIDontKnow={() => setNhsNumberSubStep(1)} />
            ) : (
                <SearchByDetailsPage onBack={() => setNhsNumberSubStep(0)} nextStep={nextStep} />
            );
    } else if (currentStepIndex === 1) {
        content = (
            <ConfirmDetailsPage
                goToConfirmCode={() => {
                    setCurrentStepIndex(2);
                    setConfirmationSubStep(1);
                }}
            />
        );
    } else if (currentStepIndex === 2) {
        content = confirmationSubStep === 0
            ? <PositiveConfirmation
                onBack={() => setCurrentStepIndex(currentStepIndex - 1)}
                goToConfirmCode={() => setConfirmationSubStep(1)}
            />
            : <ConfirmCodePage />;
    } else {
        content = stepContent[currentStepIndex].content;
    }

    return (
        <div style={{ padding: "1rem" }}>
            <h2>{stepContent[currentStepIndex].label}</h2>
            <div>{content}</div>
            {/* Only show Next button if not on NHS Number step or sub-step or Positive Confirmation or Opt In/Opt Out step */}
            {currentStepIndex !== 0 &&
                currentStepIndex !== 1 &&
                currentStepIndex !== 2 &&
                currentStepIndex !== 3 &&
                currentStepIndex < stepContent.length - 1 && (
                    <Button onClick={nextStep} className="mt-3">Next</Button>
                )}
        </div>
    );
};