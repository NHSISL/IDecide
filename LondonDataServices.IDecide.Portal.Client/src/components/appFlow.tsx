import React, { useEffect, useState } from "react";
import { ConfirmDetailsPage } from "../pages/confirmDetailsPage";
import { SearchByNhsNumberPage } from "../pages/searchByNhsNumberPage";
import { SearchByDetailsPage } from "../pages/searchByDetailsPage";
import { ConfirmCodePage } from "../pages/confirmCodePage";
import { OptInOutPage } from "../pages/optInOutPage";
import PositiveConfirmation from "./positiveConfirmation/positiveConfirmation";
import { ConfirmationPage } from "../pages/confirmationPage";
import { ThankyouPage } from "../pages/thankyouPage";
import { useStep } from "../hooks/useStep";

interface AppFlowProps {
    powerOfAttorney?: boolean;
}

export const AppFlow: React.FC<AppFlowProps> = ({ powerOfAttorney }) => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();
    const [nhsNumberSubStep, setNhsNumberSubStep] = useState(0); // 0: NHS Number, 1: Search by Details
    const [confirmationSubStep, setConfirmationSubStep] = useState(0); // 0: Positive Confirmation, 1: Confirm Code

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    // Steps array moved inside so it can use the prop
    const steps = [
        {
            key: "nhsNumber",
            label: "Provide Your NHS Number",
            subSteps: [
                {
                    key: "nhsNumberEntry",
                    label: "Enter NHS Number",
                    render: (goToSearchByDetails: () => void) => (
                        <SearchByNhsNumberPage
                            onIDontKnow={goToSearchByDetails}
                            powerOfAttorney={powerOfAttorney}
                        />
                    ),
                },
                {
                    key: "searchByDetails",
                    label: "Search By Details",
                    render: (goBack: () => void) => (
                        <SearchByDetailsPage onBack={goBack} powerOfAttorney={powerOfAttorney} />
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
            key: "confirmation",
            label: "Confirm Choices",
            render: () => <ConfirmationPage />,
        },
        {
            key: "completed",
            label: "",
            render: () => <ThankyouPage />,
        },
    ];

    let content: React.ReactNode;
    let label: string;

    switch (currentStepIndex) {
        case 0: // NHS Number step
            label = steps[0].label;
            if (nhsNumberSubStep === 0) {
                content = steps[0].subSteps?.[0]?.render?.(() => setNhsNumberSubStep(1)) ?? null;
            } else {
                content = steps[0].subSteps?.[1]?.render?.(
                    () => setNhsNumberSubStep(0)
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
                // Pass a no-op function as required
                content = steps[2].subSteps?.[1]?.render?.(() => { }) ?? null;
            }
            break;
        case 3: // Opt In/Out
            label = steps[3]?.label ?? "";
            content = steps[3]?.render?.(() => { }) ?? null;
            break;
        case 4: // Confirmation
            label = steps[4]?.label ?? "";
            content = steps[4]?.render?.(() => { }) ?? null;
            break;
        case 5: // Thank You
            label = steps[5]?.label ?? "";
            content = steps[5]?.render?.(() => { }) ?? null;
            break;
        default:
            label = "";
            content = null;
    }

    return (
        <div className="appflow-wrapper">
            <h2 className="step-label" style={{ display: "none" }}>{label}</h2>
            <div>{content}</div>
            {/* Show Next button only on steps that support it */}
            {/*{currentStepIndex === 3 && currentStepIndex < steps.length - 1 && (
                <Button onClick={goToNextMainStep} className="mt-3">Next</Button>
            )}*/}
        </div>
    );
};