import { Outlet, useLocation } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import React, { useState, useEffect, useRef } from 'react';
import { Header } from "nhsuk-react-components";
import LeftProgress from "./leftProgress/leftProgress";
import FooterComponent from "./layouts/footer";
import { useStep } from "./context/stepContext";
import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons/faPlus";
import { faMinus } from "@fortawesome/free-solid-svg-icons/faMinus";

const DEFAULT_FONT_SIZE = 16;

export default function Root() {
    const location = useLocation();

    const cleanPath = location.pathname.replace(/\/+$/, '');
    const doNotShowLeftPanelRoutes = [
        "/home",
        "/end",
      
        "/about",
        "/contact",
        "/websitePrivacyNotice",
        "/cookieUse",
        "/accessibilityStatement"];

    const doNotShowLeftPanel = doNotShowLeftPanelRoutes.includes(cleanPath);
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    const [fontSize, setFontSize] = useState(DEFAULT_FONT_SIZE);
    const [showAccessibilityBox, setShowAccessibilityBox] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const linkRef = useRef<HTMLAnchorElement>(null);

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

    // Ensure dropdown stays within viewport
    useEffect(() => {
        if (showAccessibilityBox && dropdownRef.current && linkRef.current) {
            const dropdown = dropdownRef.current;
            const link = linkRef.current;
            const dropdownRect = dropdown.getBoundingClientRect();
            const linkRect = link.getBoundingClientRect();

            // Calculate ideal left and top
            let left = linkRect.left - 50;
            let top = linkRect.bottom + 10; // Move down by 30px

            // If dropdown overflows right, shift left
            if (left + dropdownRect.width > window.innerWidth) {
                left = window.innerWidth - dropdownRect.width - 50; // 8px margin
            }
            // If dropdown overflows left, clamp to 8px
            if (left < 8) {
                left = 8;
            }

            // If dropdown overflows bottom, show above the trigger (with 30px offset)
            if (top + dropdownRect.height > window.innerHeight) {
                top = linkRect.top - dropdownRect.height - 10;
            }
            // If still off top, clamp to 8px
            if (top < 8) {
                top = 8;
            }

            dropdown.style.left = `${left}px`;
            dropdown.style.top = `${top}px`;
        }
    }, [showAccessibilityBox]);

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
                                    className="accessibility-img-link accessibility-icon-link"
                                    tabIndex={-1}
                                    ref={linkRef}
                                    style={{
                                        fontSize: "1.1em",
                                        textDecoration: "none",
                                        fontWeight: 500,
                                        cursor: "pointer",
                                        userSelect: "none",
                                        outline: "none"
                                    }}
                                    aria-haspopup="true"
                                    aria-expanded={showAccessibilityBox}
                                >
                                    <img
                                        src="/accessibility-icon-white.webp"
                                        alt="Accessibility"
                                        style={{
                                            height: "2em",
                                            verticalAlign: "middle",
                                            userSelect: "none",
                                            pointerEvents: "none"
                                        }}
                                        draggable={false}
                                    />
                                </a>
                                {showAccessibilityBox && (
                                    <div
                                        ref={dropdownRef}
                                        style={{
                                            position: "fixed",
                                            left: 0,
                                            top: 0,
                                            background: "#fff",
                                            border: "1px solid #ccc",
                                            borderRadius: "4px",
                                            boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                                            padding: "1rem 1.5rem 1rem 1rem",
                                            zIndex: 1000,
                                            minWidth: "200px"
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
                                            <FontAwesomeIcon icon={faTimes} />
                                        </button>

                                        <div style={{ display: "flex", flexDirection: "column", gap: "1em", marginTop: "1.5em" }}>
                                            <div style={{ marginBottom: "0.5em", fontWeight: 500 }}>
                                                <span>
                                                    Current Zoom &nbsp;
                                                    <span style={{ color: "#666", fontSize: "0.95em" }}>
                                                        ({Math.round(((fontSize - DEFAULT_FONT_SIZE) / DEFAULT_FONT_SIZE) * 100)}%)
                                                    </span>
                                                </span>
                                            </div>
                                            <div style={{ display: "flex", alignItems: "center", gap: "0.7em" }}>
                                                <a
                                                    href="#"
                                                    onClick={increaseFontSize}
                                                    style={{
                                                        fontSize: "1em",
                                                        textDecoration: "none",
                                                        fontWeight: 600,
                                                        color: "#005eb8",
                                                        padding: "0.2em 0.7em",
                                                        borderRadius: "4px",
                                                        border: "1px solid #005eb8",
                                                        background: "#f0f6fa"
                                                    }}
                                                    aria-label="Increase text size"
                                                >
                                                    <FontAwesomeIcon icon={faPlus} />
                                                </a>
                                                <span style={{ fontSize: "0.98em", color: "#333" }}>
                                                    Increase Text
                                                </span>
                                            </div>
                                            <div style={{ display: "flex", alignItems: "center", gap: "0.7em" }}>
                                                <a
                                                    href="#"
                                                    onClick={decreaseFontSize}
                                                    style={{
                                                        fontSize: "1em",
                                                        textDecoration: "none",
                                                        fontWeight: 600,
                                                        color: "#005eb8",
                                                        padding: "0.2em 0.7em",
                                                        borderRadius: "4px",
                                                        border: "1px solid #005eb8",
                                                        background: "#f0f6fa"
                                                    }}
                                                    aria-label="Decrease font size"
                                                >
                                                    <FontAwesomeIcon icon={faMinus} />
                                                </a>
                                                <span style={{ fontSize: "0.98em", color: "#333" }}>
                                                    Decrease Text
                                                </span>
                                            </div>

                                            <div style={{ display: "flex", alignItems: "center", gap: "0.7em" }}>
                                                <button
                                                    style={{
                                                        fontSize: "1em",
                                                        textDecoration: "none",
                                                        fontWeight: 600,
                                                        color: "#005eb8",
                                                        padding: "0.2em 0.7em",
                                                        borderRadius: "4px",
                                                        border: "1px solid #005eb8",
                                                        background: "#f0f6fa"
                                                    }}
                                                    type="button"
                                                    tabIndex={-1}
                                                    disabled
                                                >
                                                    Negative Contrast
                                                </button>
                                               
                                            </div>



                                            <div style={{ display: "flex", alignItems: "center", gap: "0.7em" }}>
                                                <a
                                                    href="#"
                                                    onClick={resetFontSize}
                                                    style={{
                                                        fontSize: "1em",
                                                        textDecoration: "none",
                                                        fontWeight: 600,
                                                        color: "#005eb8",
                                                        padding: "0.2em 0.7em",
                                                        borderRadius: "4px",
                                                        border: "1px solid #005eb8",
                                                        background: "#f0f6fa"
                                                    }}
                                                    aria-label="Reset font size"
                                                >
                                                    Reset
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </Header.Content>
                    </Header.Container>
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
                            <div className="home-content">
                                <Outlet />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>

            <FooterComponent />
        </div>
    );
}