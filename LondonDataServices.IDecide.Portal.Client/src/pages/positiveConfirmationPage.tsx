import { Breadcrumb } from "nhsuk-react-components";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Container} from "react-bootstrap";
import { useStep } from "../components/context/stepContext";

export const PositiveConfirmationPage = () => {
    const [nhsNumber, setNhsNumber] = useState("");
    const { setCurrentStepIndex } = useStep();
    const navigate = useNavigate();

    const handleNext = () => {
        setCurrentStepIndex(1); 
        navigate("/confirmNhs");
    };

    return (
        <Container fluid>
           
        </Container>
    );
};