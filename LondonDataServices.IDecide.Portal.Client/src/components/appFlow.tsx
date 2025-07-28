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

// Define all main steps and sub-steps in a clear structure
const steps = [
    {
        key: "nhsNumber",
        label: "Provide Your NHS Number",
        subSteps: [
            {
                key: "nhsNumberEntry",
                label: "Enter NHS Number",
                render: (goToSearchByDetails: () => void) => (
                    <SearchByNhsNumberPage onIDontKnow={goToSearchByDetails} />
                ),
            },
            {
                key: "searchByDetails",
                label: "Search By Details",
                render: (goBack: () => void, nextStep: () => void) => (
                    <SearchByDetailsPage onBack={goBack} nextStep={nextStep} />
                ),
            },
        ],
    },
    {
        key: "confirmDetails",
        label: "Confirm Your Details",
        render: (goToConfirmCode: () => void) => (
            <ConfirmDetailsPage goToConfirmCode={goToConfirmCode} />
        ),
    },
    {
        key: "positiveConfirmation",
        label: "Positive Confirmation",
        subSteps: [
            {
                key: "positiveConfirmation",
                label: "Positive Confirmation",
                render: (goToConfirmCode: () => void) => (
                    <PositiveConfirmation goToConfirmCode={goToConfirmCode} />
                ),
            },
            {
                key: "confirmCode",
                label: "Confirm Code",
                render: () => <ConfirmCodePage />,
            },
        ],
    },
    {
        key: "optInOut",
        label: "Make Your Choice",
        render: () => <OptInOutPage />,
    },
    {
        key: "thankYou",
        label: "Receive Notifications",
        render: () => <ThankyouPage />,
    },
];

export const AppFlow = () => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();
    const [nhsNumberSubStep, setNhsNumberSubStep] = useState(0); // 0: NHS Number, 1: Search by Details
    const [confirmationSubStep, setConfirmationSubStep] = useState(0); // 0: Positive Confirmation, 1: Confirm Code

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    const goToNextMainStep = () => {
        setCurrentStepIndex((prev: number) => Math.min(prev + 1, steps.length - 1));
        setNhsNumberSubStep(0);
        setConfirmationSubStep(0);
    };

    let content: React.ReactNode;
    let label: string;

    switch (currentStepIndex) {
        case 0: // NHS Number step
            label = steps[0].label;
            if (nhsNumberSubStep === 0) {
                content = steps[0].subSteps?.[0]?.render?.(() => setNhsNumberSubStep(1)) ?? null;
            } else {
                content = steps[0].subSteps?.[1]?.render?.(
                    () => setNhsNumberSubStep(0),
                    goToNextMainStep
                ) ?? null;
            }
            break;
        case 1: // Confirm Details
            label = steps[1].label;
            content = steps[1].render?.(() => {
                setCurrentStepIndex(2);
                setConfirmationSubStep(1);
            }) ?? null;
            break;
        case 2: // Positive Confirmation step
            label = steps[2]?.label ?? "";
            if (confirmationSubStep === 0) {
                content = steps[2].subSteps?.[0]?.render?.(
                    () => setConfirmationSubStep(1)
                ) ?? null;
            } else {
                content = steps[2].subSteps?.[1]?.render?.() ?? null;
            }
            break;
        case 3: // Opt In/Out
            label = steps[3]?.label ?? "";
            content = steps[3]?.render?.() ?? null;
            break;
        case 4: // Thank You
            label = steps[4]?.label ?? "";
            content = steps[4]?.render?.() ?? null;
            break;
        default:
            label = "";
            content = null;
    }

    return (
        <div className="appflow-wrapper">
            <h2 className="step-label">{label}</h2>
            <div>{content}</div>
            {/* Show Next button only on steps that support it */}
            {currentStepIndex === 3 && currentStepIndex < steps.length - 1 && (
                <Button onClick={goToNextMainStep} className="mt-3">Next</Button>
            )}
        </div>
    );
};