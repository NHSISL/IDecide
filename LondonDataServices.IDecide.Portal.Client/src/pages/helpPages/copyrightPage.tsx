import React from "react";
import { Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import HeaderComponent from "../../components/layouts/header";

const CopyrightPage = () => {
    const navigate = useNavigate();

    return (
        <>
            <HeaderComponent />
            <Container style={{ padding: 20, maxWidth: 800 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                    aria-label="Go back"
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    Back
                </button>
                <div style={{ marginTop: 24 }}>
                    <h1>Copyright</h1>
                    <p style={{ marginBottom: 0 }}>
                        &copy; 2025 NEL NHS<br />
                        NEL ICB<br />
                        Unex Tower<br />
                        5 Station Street<br />
                        London<br />
                        E15 1DA
                    </p>
                </div>
            </Container>
        </>
    );
};

export default CopyrightPage;