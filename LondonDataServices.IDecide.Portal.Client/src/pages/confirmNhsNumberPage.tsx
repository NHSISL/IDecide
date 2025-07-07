import { Breadcrumb } from "nhsuk-react-components";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Container } from "react-bootstrap";
import { useStep } from "../components/context/stepContext";
import { StepWizard } from "../components/stepWizard";

export const ConfirmNhsNumber = () => {
    const [nhsNumber, setNhsNumber] = useState("");
    const { setCurrentStepIndex } = useStep();
    const navigate = useNavigate();

    const handleNext = () => {
        setCurrentStepIndex(1); // Step index for "Confirm Your Details"
        navigate("/confirmDetails"); // ✅ no query param
    };

    return (
        <Container fluid>
            {/*<div>*/}
            {/*    <label>*/}
            {/*        NHS Number:*/}
            {/*        <input*/}
            {/*            type="text"*/}
            {/*            value={nhsNumber}*/}
            {/*            onChange={(e) => setNhsNumber(e.target.value)}*/}
            {/*            placeholder="Enter NHS number"*/}
            {/*        />*/}
            {/*    </label>*/}
            {/*    <button onClick={handleNext}>*/}
            {/*        Search*/}
            {/*    </button>*/}
            {/*</div>*/}

            <StepWizard></StepWizard>
        </Container>
    );
};
