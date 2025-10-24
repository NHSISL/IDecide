import React, { createContext, useState, ReactNode } from "react";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttorney } from "../../models/powerOfAttourneys/powerOfAttourney";

type StepContextType = {
    currentStepIndex: number;
    setCurrentStepIndex: React.Dispatch<React.SetStateAction<number>>;
    nextStep: (
        selectedOption?: "optout" | "optin",
        nhsNumber?: string,
        patient?: Patient,
        powerOfAttorney?: PowerOfAttorney
    ) => void;
    previousStep: (
        selectedOption?: "optout" | "optin",
        nhsNumber?: string,
        patient?: Patient,
        powerOfAttorney?: PowerOfAttorney
    ) => void;
    createdPatient: Patient | null;
    setCreatedPatient: React.Dispatch<React.SetStateAction<Patient | null>>;
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
    powerOfAttorney: PowerOfAttorney | null;
    setPowerOfAttorney: React.Dispatch<React.SetStateAction<PowerOfAttorney | null>>;
    resetStepContext: () => void;
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
    const [powerOfAttorney, setPowerOfAttorney] = useState<PowerOfAttorney | null>(null);

    const nextStep = (
        option?: "optout" | "optin",
        nhs?: string,
        patient?: Patient,
        poa?: PowerOfAttorney
    ) => {
        if (option) setSelectedOption(option);
        if (nhs) setNhsNumber(nhs);
        if (patient) setCreatedPatient(patient);
        if (poa) setPowerOfAttorney(poa);
        setCurrentStepIndex((i) => i + 1);
    };

    const previousStep = (
        option?: "optout" | "optin",
        nhs?: string,
        patient?: Patient,
        poa?: PowerOfAttorney
    ) => {
        if (option) setSelectedOption(option);
        if (nhs) setNhsNumber(nhs);
        if (patient) setCreatedPatient(patient);
        if (poa) setPowerOfAttorney(poa);
        setCurrentStepIndex((i) => (i > 0 ? i - 1 : 0));
    };

    const resetStepContext = () => {
        setCurrentStepIndex(0);
        setCreatedPatient(null);
        setSelectedOption(null);
        setNhsNumber(null);
        setPowerOfAttorney(null);
    };

    return (
        <StepContext.Provider
            value={{
                currentStepIndex,
                setCurrentStepIndex,
                nextStep,
                previousStep,
                createdPatient,
                setCreatedPatient,
                selectedOption,
                nhsNumber,
                powerOfAttorney,
                setPowerOfAttorney,
                resetStepContext
            }}
        >
            {children}
        </StepContext.Provider>
    );
};

export { StepContext };