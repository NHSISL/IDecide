import React, { useState, useEffect, useRef } from "react";
import { Header } from "nhsuk-react-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { faPlus } from "@fortawesome/free-solid-svg-icons/faPlus";
import { faMinus } from "@fortawesome/free-solid-svg-icons/faMinus";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

const DEFAULT_FONT_SIZE = 16;

const HeaderComponent: React.FC = () => {
    const [fontSize, setFontSize] = useState(DEFAULT_FONT_SIZE);
    const [showAccessibilityBox, setShowAccessibilityBox] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const linkRef = useRef<HTMLAnchorElement>(null);

    const { configuration } = useFrontendConfiguration();

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

    useEffect(() => {
        if (showAccessibilityBox && dropdownRef.current && linkRef.current) {
            const dropdown = dropdownRef.current;
            const link = linkRef.current;
            const dropdownRect = dropdown.getBoundingClientRect();
            const linkRect = link.getBoundingClientRect();

            let left = linkRect.left - 50;
            let top = linkRect.bottom + 10;

            if (left + dropdownRect.width > window.innerWidth) {
                left = window.innerWidth - dropdownRect.width - 50;
            }
            if (left < 8) {
                left = 8;
            }
            if (top + dropdownRect.height > window.innerHeight) {
                top = linkRect.top - dropdownRect.height - 10;
            }
            if (top < 8) {
                top = 8;
            }

            dropdown.style.left = `${left}px`;
            dropdown.style.top = `${top}px`;
        }
    }, [showAccessibilityBox]);

    useEffect(() => {
        if (configuration?.bannerColour) {
            document.documentElement.style.setProperty('--nhsuk-header-bg', configuration.bannerColour);
        }
    }, [configuration?.bannerColour]);

    return (
        <>
            {configuration?.environment && configuration.environment !== "Production" && (
                <div
                    style={{
                        position: "fixed",
                        top: 0,
                        left: 0,
                        width: "100vw",
                        height: "20px",
                        background: "red",
                        zIndex: 2000,
                        display: "flex",
                        alignItems: "center",     
                        justifyContent: "center",
                    }}
                >
                    <div className="test-env-banner">
                        <small>
                            <strong>Test Environment:</strong> This website is for demonstration and testing purposes only. Any information you enter here will not be saved to real patient records or used for clinical care. Please do not enter real patient data.
                        </small>
                    </div>
                </div>
            )}
            <div
                className={
                    configuration?.environment && configuration.environment !== "Production"
                        ? "header-padding-top"
                        : undefined
                }
            >
                <Header style={{ backgroundColor: configuration?.bannerColour || undefined }}>
                    <Header.Container>
                        <Header.Logo href="/home" />
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

                                    <span className="me-4 text-white">
                                        {configuration?.environment}
                                    </span>

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

                                            {/*<div style={{ display: "flex", alignItems: "center", gap: "0.7em" }}>*/}
                                            {/*    <button*/}
                                            {/*        style={{*/}
                                            {/*            fontSize: "1em",*/}
                                            {/*            textDecoration: "none",*/}
                                            {/*            fontWeight: 600,*/}
                                            {/*            color: "#005eb8",*/}
                                            {/*            padding: "0.2em 0.7em",*/}
                                            {/*            borderRadius: "4px",*/}
                                            {/*            border: "1px solid #005eb8",*/}
                                            {/*            background: "#f0f6fa"*/}
                                            {/*        }}*/}
                                            {/*        type="button"*/}
                                            {/*        tabIndex={-1}*/}
                                            {/*        disabled*/}
                                            {/*    >*/}
                                            {/*        Negative Contrast*/}
                                            {/*    </button>*/}
                                            {/*</div>*/}

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
            </div>
        </>
    );
};

export default HeaderComponent;