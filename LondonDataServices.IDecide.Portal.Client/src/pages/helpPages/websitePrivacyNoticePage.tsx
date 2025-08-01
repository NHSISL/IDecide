﻿import React from "react";
import { Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import HeaderComponent from "../../components/layouts/header";

const WebsitePrivacyNoticePage = () => {
    const navigate = useNavigate();

    return (
        <>
            <HeaderComponent />
            <Container style={{ padding: 20 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    Back
                </button>
                <div style={{ marginTop: 24 }}>
                    This is some Website Privacy Notice information.
                </div>
            </Container>
        </>
    );
};

export default WebsitePrivacyNoticePage;