import React, { createContext, useContext, useState, ReactNode } from "react";
import { Patient } from "../../models/patients/patient";

type StepContextType = {
    currentStepIndex: number;
    setCurrentStepIndex: React.Dispatch<React.SetStateAction<number>>;
    nextStep: (selectedOption?: "optout" | "optin", nhsNumber?: string, patient?: Patient) => void;
    createdPatient: Patient | null;
    setCreatedPatient: React.Dispatch<React.SetStateAction<Patient | null>>;
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
};

const StepContext = createContext<StepContextType | undefined>(undefined);

interface StepProviderProps {
    children: ReactNode;
}

export const StepProvider = ({ children }: StepProviderProps) => {
    const [currentStepIndex, setCurrentStepIndex] = useState(0);
    const [createdPatient, setCreatedPatient] = useState<Patient | null>(null);
    const [selectedOption, setSelectedOption] = useState<"optout" | "optin" | null>(null);
    const [nhsNumber, setNhsNumber] = useState<string | null>(null);

    // nextStep can be called with or without params, depending on the step
    const nextStep = (
        option?: "optout" | "optin",
        nhs?: string,
        patient?: Patient
    ) => {
        if (option) setSelectedOption(option);
        if (nhs) setNhsNumber(nhs);
        if (patient) setCreatedPatient(patient);
        setCurrentStepIndex((i) => i + 1);
    };

    return (
        <StepContext.Provider
            value={{
                currentStepIndex,
                setCurrentStepIndex,
                nextStep,
                createdPatient,
                setCreatedPatient,
                selectedOption,
                nhsNumber,
            }}
        >
            {children}
        </StepContext.Provider>
    );
};

export const useStep = () => {
    const context = useContext(StepContext);
    if (!context) {
        throw new Error("useStep must be used within a StepProvider");
    }
    return context;
};