import React from "react";
import { useStep } from "../../hooks/useStep";
import { useNavigate } from "react-router-dom";
import { Button } from "react-bootstrap";
import { useTranslation } from "react-i18next";

export const Login = () => {
    const { setCurrentStepIndex } = useStep();

    const handleNext = () => {
        setCurrentStepIndex((prev: number) => prev + 1);
    };

    const { t: translate } = useTranslation();
    const navigate = useNavigate();

    return (
        <div>
            Login page
            <Button
                onClick={() => navigate("/optOut", { state: { powerOfAttorney: false } })}
                style={{ margin: "0 0 1rem 1rem", width: 260, fontWeight: 600, minHeight: 75 }}>
                Login
            </Button>
        </div>
    );
};

export default Login;