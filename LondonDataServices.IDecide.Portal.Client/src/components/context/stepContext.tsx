import React, { createContext, useContext, useState } from "react";

const StepContext = createContext<any>(null);

export const StepProvider = ({ children }) => {
    const [currentStepIndex, setCurrentStepIndex] = useState(0);
    const [nhsNumber, setNhsNumber] = useState(""); // NHS Number state

    const nextStep = () => setCurrentStepIndex((i) => i + 1);

    return (
        <StepContext.Provider
            value={{
                currentStepIndex,
                setCurrentStepIndex,
                nextStep,
                nhsNumber,
                setNhsNumber,
            }}
        >
            {children}
        </StepContext.Provider>
    );
};

export const useStep = () => useContext(StepContext);