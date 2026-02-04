import React, { useState, useEffect, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes, faPlus, faMinus } from "@fortawesome/free-solid-svg-icons";

const DEFAULT_FONT_SIZE = 16;

const AccessibilityBox: React.FC = () => {
    const [fontSize, setFontSize] = useState(DEFAULT_FONT_SIZE);
    const [showAccessibilityBox, setShowAccessibilityBox] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const linkRef = useRef<HTMLAnchorElement>(null);

    useEffect(() => {
        document.documentElement.style.setProperty('--app-font-size', `${fontSize}px`);
    }, [fontSize]);

    useEffect(() => {
        if (!showAccessibilityBox) return;

        const handleClickOutside = (event: MouseEvent) => {
            if (
                dropdownRef.current &&
                !dropdownRef.current.contains(event.target as Node) &&
                linkRef.current &&
                !linkRef.current.contains(event.target as Node)
            ) {
                setShowAccessibilityBox(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);

        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [showAccessibilityBox]);

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

    return (
        <div style={{
            position: "fixed",
            bottom: "1em",
            right: "0.5em",
            zIndex: "1100",
            background: "transparent"
        }}>
            <a
                href="#"
                onClick={toggleAccessibilityBox}
                className="accessibility-img-link accessibility-icon-link"
                tabIndex={-1}
                ref={linkRef}
                style={{
                    fontSize: "1.5em",
                    textDecoration: "none",
                    fontWeight: 500,
                    userSelect: "none",
                    outline: "none",
                }}
                aria-haspopup="true"
                aria-expanded={showAccessibilityBox}
            >
                <img
                    src="/64px-Accessibility.svg.png"
                    alt="Accessibility"
                    className="accessibility-img"
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
                        background: "#f9fbfc",
                        border: "1px solid #e3e7ea",
                        borderRadius: "10px",
                        boxShadow: "0 4px 24px rgba(0,0,0,0.08)",
                        padding: "0.75rem 1.25rem 0.75rem 1rem",
                        zIndex: 1000,
                        minWidth: "220px",
                        transition: "box-shadow 0.2s, border 0.2s, background 0.2s"
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
                            color: "#888"
                        }}
                        aria-label="Close accessibility options"
                    >
                        <FontAwesomeIcon icon={faTimes} />
                    </button>
                    <div style={{ display: "flex", flexDirection: "column", gap: "0.7em", marginTop: "1.2em" }}>
                        <div style={{ marginBottom: "0.3em", fontWeight: 500, fontSize: "1em", color: "#222" }}>
                            <span>
                                Current Zoom&nbsp;
                                <span style={{ color: "#666", fontSize: "0.95em" }}>
                                    ({Math.round(((fontSize - DEFAULT_FONT_SIZE) / DEFAULT_FONT_SIZE) * 100)}%)
                                </span>
                            </span>
                        </div>
                        <div style={{ display: "flex", alignItems: "center", gap: "0.5em" }}>
                            <button
                                onClick={increaseFontSize}
                                style={{
                                    fontSize: "1em",
                                    textDecoration: "none",
                                    fontWeight: 600,
                                    color: "#005eb8",
                                    padding: "0.15em 0.6em",
                                    borderRadius: "6px",
                                    border: "1px solid #d1e3f0",
                                    background: "#f4f8fb",
                                    boxShadow: "0 1px 2px rgba(0,0,0,0.03)",
                                    transition: "background 0.2s, border 0.2s"
                                }}
                                aria-label="Increase text size"
                            >
                                <FontAwesomeIcon icon={faPlus} />
                            </button>
                            <span style={{ fontSize: "0.97em", color: "#333" }}>
                                Increase Text
                            </span>
                        </div>
                        <div style={{ display: "flex", alignItems: "center", gap: "0.5em" }}>
                            <button
                                onClick={decreaseFontSize}
                                style={{
                                    fontSize: "1em",
                                    textDecoration: "none",
                                    fontWeight: 600,
                                    color: "#005eb8",
                                    padding: "0.15em 0.6em",
                                    borderRadius: "6px",
                                    border: "1px solid #d1e3f0",
                                    background: "#f4f8fb",
                                    boxShadow: "0 1px 2px rgba(0,0,0,0.03)",
                                    transition: "background 0.2s, border 0.2s"
                                }}
                                aria-label="Decrease font size"
                            >
                                <FontAwesomeIcon icon={faMinus} />
                            </button>
                            <span style={{ fontSize: "0.97em", color: "#333" }}>
                                Decrease Text
                            </span>
                        </div>
                        <div style={{ display: "flex", alignItems: "center", gap: "0.5em" }}>
                            <button
                                onClick={resetFontSize}
                                style={{
                                    fontSize: "1em",
                                    textDecoration: "none",
                                    fontWeight: 600,
                                    color: "#005eb8",
                                    padding: "0.15em 0.6em",
                                    borderRadius: "6px",
                                    border: "1px solid #d1e3f0",
                                    background: "#f4f8fb",
                                    boxShadow: "0 1px 2px rgba(0,0,0,0.03)",
                                    transition: "background 0.2s, border 0.2s"
                                }}
                                aria-label="Reset font size"
                            >
                                Reset
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AccessibilityBox;