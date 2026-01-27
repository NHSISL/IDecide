import React, { useEffect } from "react";
import { OptInOutPage } from "../pages/optInOutPage";
import { ThankyouPage } from "../pages/thankyouPage";
import { useStep } from "../hooks/useStep";
import NhsLoginOptOutPage from "../pages/nhsLoginOptOut";
import ConfirmationNhsLoginPage from "../pages/confirmationNhsLoginPage";


export const AppFlowNhsLogin: React.FC = () => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    useEffect(() => {
        setCurrentStepIndex(0);
    }, [setCurrentStepIndex]);

    const steps = [
        {
            key: "PositiveConfirmation",
            label: "Make Your Choice",
            render: () => <NhsLoginOptOutPage />,
        },
        {
            key: "optInOut",
            label: "Make Your Choice",
            render: () => <OptInOutPage />,
        },
        {
            key: "confirmation",
            label: "Confirm Choices",
            render: () => <ConfirmationNhsLoginPage />,
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
        case 0:
            label = steps[0].label;
            content = steps[0].render();
            break;
        case 1:
            label = steps[1].label;
            content = steps[1].render();
            break;
        case 2:
            label = steps[2].label;
            content = steps[2].render();
            break;
        case 3:
            label = steps[3].label;
            content = steps[3].render();
            break;
        default:
            label = "";
            content = null;
    }

    return (
        <div className="appflow-wrapper">
            <h2 className="step-label" style={{ display: "none" }}>{label}</h2>
            <div>{content}</div>
        </div>
    );
};