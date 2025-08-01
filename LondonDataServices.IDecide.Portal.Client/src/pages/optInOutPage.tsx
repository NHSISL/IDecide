import React from "react";
import { Container } from "react-bootstrap";
import OptInOptOut from "../components/optInOptOut/optInOptOut";
import { useStep } from "../components/context/stepContext";

export const OptInOutPage = () => {
    const { createdPatient } = useStep();
    return (
        <Container style={{ padding: 20 }}>
            <OptInOptOut createdPatient={createdPatient} />
        </Container>
    );
};