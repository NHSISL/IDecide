import { Outlet, useLocation } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import React, { useState, useEffect } from 'react';
import { Footer, Header } from "nhsuk-react-components";
import LeftProgress from "./leftProgress/leftProgress";
import { useStep } from "./context/stepContext";

const DEFAULT_FONT_SIZE = 16;

export default function Root() {
    const location = useLocation();

    const cleanPath = location.pathname.replace(/\/+$/, '');
    const doNotShowLeftPanelRoutes = ["/home", "/end"];
    const doNotShowLeftPanel = doNotShowLeftPanelRoutes.includes(cleanPath);
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    const [fontSize, setFontSize] = useState(DEFAULT_FONT_SIZE);
    const [showAccessibilityBox, setShowAccessibilityBox] = useState(false);

    useEffect(() => {
        document.documentElement.style.setProperty('--app-font-size', `${fontSize}px`);
    }, [fontSize]);

    const increaseFontSize = (e: React.MouseEvent) => {
        e.preventDefault();
        setFontSize((size) => size + 2);
    };

    const decreaseFontSize = (e: React.MouseEvent) => {
        e.preventDefault();
        setFontSize((size) => Math.max(8, size - 2));
    };

    const resetFontSize = (e: React.MouseEvent) => {
        e.preventDefault();
        setFontSize(DEFAULT_FONT_SIZE);
    };

    const toggleAccessibilityBox = (e: React.MouseEvent) => {
        e.preventDefault();
        setShowAccessibilityBox((show) => !show);
    };

    const closeAccessibilityBox = (e: React.MouseEvent) => {
        e.preventDefault();
        setShowAccessibilityBox(false);
    };

    return (
        <div className="root-layout">
            {!doNotShowLeftPanel && (
                <Header>
                    <Header.Container>
                        <Header.Logo href="/" />
                        <Header.Content>
                            <div style={{ position: "relative", display: "inline-block" }}>
                                <a
                                    href="#"
                                    onClick={toggleAccessibilityBox}
                                    style={{
                                        fontSize: "1.1em",
                                        textDecoration: "none",
                                        fontWeight: 500,
                                        cursor: "pointer"
                                    }}
                                    aria-haspopup="true"
                                    aria-expanded={showAccessibilityBox}
                                >
                                    Accessibility
                                </a>
                                {showAccessibilityBox && (
                                    <div
                                        style={{
                                            position: "absolute",
                                            top: "2.2em",
                                            left: 0,
                                            background: "#fff",
                                            border: "1px solid #ccc",
                                            borderRadius: "4px",
                                            boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                                            padding: "1rem 1.5rem 1rem 1rem",
                                            zIndex: 1000,
                                            minWidth: "180px"
                                        }}
                                        role="dialog"
                                        aria-label="Accessibility options"
                                    >
                                        <button
                                            onClick={closeAccessibilityBox}
                                            style={{
                                                position: "absolute",
                                                top: "0.5em",
                                                right: "0.7em",
                                                background: "none",
                                                border: "none",
                                                fontSize: "1.2em",
                                                cursor: "pointer",
                                                color: "#666"
                                            }}
                                            aria-label="Close accessibility options"
                                        >
                                            ×
                                        </button>
                                        <div style={{ display: "flex", flexDirection: "column", gap: "0.5em", marginTop: "1.5em" }}>
                                            <a
                                                href="#"
                                                onClick={increaseFontSize}
                                                style={{ fontSize: "1.1em", textDecoration: "none" }}
                                                aria-label="Increase font size"
                                            >
                                                A+
                                            </a>
                                            <a
                                                href="#"
                                                onClick={decreaseFontSize}
                                                style={{ fontSize: "1.1em", textDecoration: "none" }}
                                                aria-label="Decrease font size"
                                            >
                                                A-
                                            </a>
                                            <a
                                                href="#"
                                                onClick={resetFontSize}
                                                style={{ fontSize: "1.1em", textDecoration: "none" }}
                                                aria-label="Reset font size"
                                            >
                                                Reset
                                            </a>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </Header.Content>
                    </Header.Container>
                    <Header.Nav>
                        {/* Optional nav */}
                    </Header.Nav>
                </Header>
            )}

            <div className="root-content">
                <Container>
                    <Row>
                        {!doNotShowLeftPanel && (
                            <Col md={5}>
                                <LeftProgress
                                    currentStepIndex={currentStepIndex}
                                    setCurrentStepIndex={setCurrentStepIndex}
                                />
                            </Col>
                        )}
                        <Col md={doNotShowLeftPanel ? 12 : 7}>
                            <Outlet />
                        </Col>
                    </Row>
                </Container>
            </div>

            <Footer />
        </div>
    );
}