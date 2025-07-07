import React, { createContext, useContext, useState } from "react";

interface StepContextType {
    currentStepIndex: number;
    setCurrentStepIndex: React.Dispatch<React.SetStateAction<number>>;
}

const StepContext = createContext<StepContextType | undefined>(undefined);

export const StepProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [currentStepIndex, setCurrentStepIndex] = useState(0); // Start at step 0

    return (
        <StepContext.Provider value={{ currentStepIndex, setCurrentStepIndex }}>
            {children}
        </StepContext.Provider>
    );
};

export const useStep = () => {
    const context = useContext(StepContext);
    if (!context) {
        throw new Error("useStep must be used within StepProvider");
    }
    return context;
};
