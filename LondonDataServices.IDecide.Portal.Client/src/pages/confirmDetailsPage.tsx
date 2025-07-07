import { Breadcrumb } from "nhsuk-react-components";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Container} from "react-bootstrap";
import { useStep } from "../components/context/stepContext";

export const ConfirmDetailsPage = () => {
    const { setCurrentStepIndex } = useStep();
    const navigate = useNavigate();

    const handleNext = () => {
        setCurrentStepIndex(2);
        navigate("/positiveConfirmation");
    };

    return (
        <Container fluid>
            <div>
                <button onClick={handleNext}>
                    Next Page
                </button>
            </div>
        </Container>
    );
};