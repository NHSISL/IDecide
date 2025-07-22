/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { createContext, useContext, useState, ReactNode } from "react";
import { Patient } from "../../models/patients/patient";

const StepContext = createContext<any>(null);

interface StepProviderProps {
    children: ReactNode;
}

export const StepProvider = ({ children }: StepProviderProps) => {
    const [currentStepIndex, setCurrentStepIndex] = useState(0);
    const [createdPatient, setCreatedPatient] = useState<Patient | null>(null);

    const nextStep = () => setCurrentStepIndex((i) => i + 1);

    return (
        <StepContext.Provider
            value={{
                currentStepIndex,
                setCurrentStepIndex,
                nextStep,
                createdPatient,
                setCreatedPatient,
            }}
        >
            {children}
        </StepContext.Provider>
    );
};

export const useStep = () => useContext(StepContext);