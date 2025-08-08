import React, { createContext, useState, ReactNode } from "react";
import { Patient } from "../../models/patients/patient";
import { PowerOfAttourney } from "../../models/powerOfAttourneys/powerOfAttourney";

type StepContextType = {
    currentStepIndex: number;
    setCurrentStepIndex: React.Dispatch<React.SetStateAction<number>>;
    nextStep: (
        selectedOption?: "optout" | "optin",
        nhsNumber?: string,
        patient?: Patient,
        powerOfAttourney?: PowerOfAttourney
    ) => void;
    createdPatient: Patient | null;
    setCreatedPatient: React.Dispatch<React.SetStateAction<Patient | null>>;
    selectedOption: "optout" | "optin" | null;
    nhsNumber: string | null;
    powerOfAttourney: PowerOfAttourney | null;
    setPowerOfAttourney: React.Dispatch<React.SetStateAction<PowerOfAttourney | null>>;
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
    const [powerOfAttourney, setPowerOfAttourney] = useState<PowerOfAttourney | null>(null);

    const nextStep = (
        option?: "optout" | "optin",
        nhs?: string,
        patient?: Patient,
        poa?: PowerOfAttourney
    ) => {
        console.log("nextStep called with:", { option, nhs, patient, poa });
        if (option) setSelectedOption(option);
        if (nhs) setNhsNumber(nhs);
        if (patient) setCreatedPatient(patient);
        if (poa) setPowerOfAttourney(poa);
        setCurrentStepIndex((i) => i + 1);
    };

    // Add a reset function
    const resetStepContext = () => {
        setCurrentStepIndex(0);
        setCreatedPatient(null);
        setSelectedOption(null);
        setNhsNumber(null);
        setPowerOfAttourney(null);
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
                powerOfAttourney,
                setPowerOfAttourney,
                resetStepContext
            }}
        >
            {children}
        </StepContext.Provider>
    );
};

export { StepContext };