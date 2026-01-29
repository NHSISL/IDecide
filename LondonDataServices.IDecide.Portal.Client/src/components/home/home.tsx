import React, { useState } from "react";
import { Button } from "nhsuk-react-components";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlusCircle, faMinusCircle } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import LanguageSelector from "../languageSwitcher/languageSelector";

interface ExpandableSectionProps {
    header: string;
    content: React.ReactNode[];
    expanded: boolean;
    onClick: () => void;
    id: string;
}

const ExpandableSection: React.FC<ExpandableSectionProps> = ({
    header,
    content,
    expanded,
    onClick,
    id
}) => (
    <div
        className="expandable-section"
        style={{
            marginBottom: "1rem",
            border: "1px solid #d8dde0",
            borderRadius: "6px",
            background: "#fff",
            boxShadow: expanded ? "0 2px 8px rgba(0,94,184,0.08)" : "none",
            transition: "box-shadow 0.2s"
        }}
    >
        <button
            type="button"
            onClick={onClick}
            aria-expanded={expanded}
            aria-controls={id}
            style={{
                background: expanded ? "#e8f4fa" : "#f0f4f5",
                border: "none",
                width: "100%",
                textAlign: "left",
                fontWeight: 600,
                fontSize: "1rem",
                padding: "1rem",
                cursor: "pointer",
                display: "flex",
                alignItems: "center",
                borderRadius: "6px 6px 0 0",
                outline: "none"
            }}
        >
            <span style={{ marginRight: "0.75rem" }}>
                <FontAwesomeIcon
                    icon={expanded ? faMinusCircle : faPlusCircle}
                    color="#005eb8"
                    style={{ fontSize: "24px" }}
                />
            </span>
            {header}
        </button>
        <div
            id={id}
            style={{
                maxHeight: expanded ? 500 : 0,
                overflow: "hidden",
                transition: "max-height 0.3s ease",
                background: "#f9fafb",
                padding: expanded ? "1rem" : "0 1rem",
                borderTop: expanded ? "1px solid #d8dde0" : "none",
                borderRadius: "0 0 6px 6px"
            }}
            aria-hidden={!expanded}
        >
            {expanded && content}
        </div>
    </div>
);

export const Home = () => {
    const { t: translate } = useTranslation();
    const navigate = useNavigate();
    // Track expanded state for each section
    const [expandedSections, setExpandedSections] = useState<boolean[]>([false, false, false, false]);

    const expandableHeaders = [
        translate("homepage.expandableHeaders.section1"),
        translate("homepage.expandableHeaders.section2"),
        translate("homepage.expandableHeaders.section3"),
        translate("homepage.expandableHeaders.section4")
    ];

    const expandableContent: React.ReactNode[][] = [
        [
            <p key="p1">{translate("homepage.expandableContent.section1.p1")}</p>,
            <p key="p2">{translate("homepage.expandableContent.section1.p2")}</p>
        ],
        [
            <p key="p1">{translate("homepage.expandableContent.section2.p1")}</p>,
            <p key="p2">{translate("homepage.expandableContent.section2.p2")}</p>
        ],
        [
            <p key="p1">{translate("homepage.expandableContent.section3.p1")}</p>,
            <p key="p2">
                {translate("homepage.expandableContent.section3.p2").split("NELondonicb.oneLondon.opt-out@nhs.net")[0]}
                <a href="mailto:NELondonicb.oneLondon.opt-out@nhs.net">NELondonicb.oneLondon.opt-out@nhs.net</a>
                {translate("homepage.expandableContent.section3.p2").split("NELondonicb.oneLondon.opt-out@nhs.net")[1]}
            </p>
        ],
        [
            <p key="p3">
                {translate("homepage.expandableContent.section4.p1")}{' '}
                <a
                    href="https://healthinnovationnetwork.com/wp-content/uploads/2025/07/OLHDS-LAP-Full-Privacy-Notice.pdf"
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    {translate("homepage.expandableContent.section4.p1LinkText")}
                </a>{' '}
                {translate("homepage.expandableContent.section4.p2")}{' '}
                <a
                    href="https://healthinnovationnetwork.com/wp-content/uploads/2025/07/OLHDS-LDS-Full-Privacy-Notice.pdf"
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    {translate("homepage.expandableContent.section4.p2LinkText")}
                </a>.
            </p>
        ]
    ];

    const handleSectionClick = (idx: number) => {
        setExpandedSections(prev => {
            const updated = [...prev];
            updated[idx] = !updated[idx];
            return updated;
        });
    };

    return (
        <div className="home-content" style={{ padding: "1.5rem 0.5rem" }}>
            <div className="home-box" style={{ maxWidth: 1700, margin: "0 auto" }}>
                {/* Language Selector (hidden for now) */}
                <div style={{ position: "absolute", top: 5, right: 5, display: "none" }}>
                    <LanguageSelector />
                </div>

                {/* Title and Introduction */}
                <header>
                    <h1 style={{ fontSize: "1.7rem", marginBottom: "0.7rem" }}>
                        {translate("homepage.title")}
                    </h1>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro1")}</p>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro2")}</p>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro3")}</p>
                </header>

                {/* Before You Start Section */}
                <section
                    style={{
                        marginTop: "1rem",
                        background: "#f0f4f5",
                        padding: "0.7rem 1rem",
                        borderRadius: "6px",
                        fontSize: "0.97rem"
                    }}
                >
                    <strong>{translate("homepage.beforeYouStartTitle")}</strong>
                    <ul style={{ marginTop: "0.3rem", paddingLeft: "1.1rem" }}>
                        <li>{translate("homepage.beforeYouStartList1")}</li>
                        <li>{translate("homepage.beforeYouStartList2")}</li>
                    </ul>
                </section>

                <section style={{ margin: "1rem 0 0 0rem" }}>
                    <p style={{ marginBottom: "0.7rem" }}>
                        <strong>{translate("homepage.intro4")}</strong>
                    </p>
                    <div style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
                        <Button
                            className="nhsuk-button-blue"
                            onClick={() => (window.location.href = "/login")}
                            style={{
                                width: 260,
                                fontWeight: 600,
                                minHeight: 75
                            }}
                        >
                            {translate("homepage.startButton")}
                        </Button>
                        <Button
                            onClick={() => {
                                fetch("/logout", { method: "POST" }).then(d => {
                                    if (d.ok) {
                                        navigate("/optOut", { state: { powerOfAttorney: true } });
                                    }
                                });
                            }}
                            style={{
                                width: 260,
                                fontWeight: 600,
                                minHeight: 75
                            }}
                        >
                            {translate("homepage.startButtonOther")}
                        </Button>
                    </div>
                </section>

                {/* Expandable Sections */}
                <section>
                    {expandableHeaders.map((header, idx) => (
                        <ExpandableSection
                            key={header}
                            header={header}
                            content={expandableContent[idx]}
                            expanded={expandedSections[idx]}
                            onClick={() => handleSectionClick(idx)}
                            id={`expandable-section-${idx}`}
                        />
                    ))}
                </section>
            </div>
        </div>
    );
};

export default Home;