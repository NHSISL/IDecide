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
}

const ExpandableSection: React.FC<ExpandableSectionProps> = ({
    header,
    content,
    expanded,
    onClick
}) => (
    <div className="expandable-section" style={{ marginBottom: "0.5rem" }}>
        <button
            type="button"
            onClick={onClick}
            style={{
                background: "none",
                border: "none",
                width: "100%",
                textAlign: "left",
                fontWeight: 600,
                fontSize: "0.98rem",
                padding: "0.3rem 0",
                cursor: "pointer",
                display: "flex",
                alignItems: "center"
            }}
            aria-expanded={expanded}>

            <span style={{ marginRight: "0.5rem" }}>
                <FontAwesomeIcon
                    icon={expanded ? faMinusCircle : faPlusCircle}
                    color="black"
                    style={{ fontSize: "22px" }} />
            </span>
            {header}
        </button>
        {expanded && (
            <div style={{ padding: "0.3rem 0 0.5rem 2rem", fontSize: "0.97rem" }}>
                {content}
            </div>
        )}
    </div>
);

export const Home = () => {
    const { t: translate } = useTranslation();
    const navigate = useNavigate();
    const [expandedIndex, setExpandedIndex] = useState<number | null>(null);

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
                <a href="https://healthinnovationnetwork.com/wp-content/uploads/2025/07/OLHDS-LAP-Full-Privacy-Notice.pdf" target="_blank" rel="noopener noreferrer">
                    {translate("homepage.expandableContent.section4.p1LinkText")}
                </a>{' '}
                {translate("homepage.expandableContent.section4.p2")}{' '}
                <a href="https://healthinnovationnetwork.com/wp-content/uploads/2025/07/OLHDS-LDS-Full-Privacy-Notice.pdf" target="_blank" rel="noopener noreferrer">
                    {translate("homepage.expandableContent.section4.p2LinkText")}
                </a>.
            </p>
        ]
    ];

    return (
        <div className="home-content" style={{ padding: "1.5rem 0.5rem" }}>
            <div className="home-box" style={{ maxWidth: 1700, margin: "0 auto", borderRadius: 8, boxShadow: "0 2px 8px #e0e0e0", padding: "2rem 1.5rem", background: "rgba(255, 255, 255, 0.70)" }}>

                <div style={{ position: "absolute", top: 5, right: 5, display: "none" }}>
                    <LanguageSelector />
                </div>

                <h1 style={{ fontSize: "1.7rem", marginBottom: "0.7rem" }}>{translate("homepage.title")}</h1>
                <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro1")}</p>
                <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro2")}</p>
                <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro3")}</p>
                <p style={{ marginBottom: "0.7rem" }}>
                    <strong>{translate("homepage.intro4")}</strong>
                </p>
                <Button className="nhsuk-button-blue"
                    onClick={() => window.location.href = "/login" }
                    style={{ margin: "0 0 1rem 1rem", width: 260, fontWeight: 600, minHeight: 75 }}>
                    {translate("homepage.startButton")}
                </Button>

                <Button
                    onClick={() => navigate("/optOut", { state: { powerOfAttorney: true } })}
                    style={{ margin: "0 0 1rem 1rem", width: 260, fontWeight: 600, minHeight: 75 }}>
                    {translate("homepage.startButtonOther")}
                </Button>

                {expandableHeaders.map((header, idx) => (
                    <ExpandableSection
                        key={header}
                        header={header}
                        content={expandableContent[idx]}
                        expanded={expandedIndex === idx}
                        onClick={() => setExpandedIndex(expandedIndex === idx ? null : idx)}
                    />
                ))}
                <div style={{ marginTop: "1rem", background: "#f0f4f5", padding: "0.7rem 1rem", borderRadius: "6px", fontSize: "0.97rem" }}>
                    <strong>{translate("homepage.beforeYouStartTitle")}</strong>
                    <ul style={{ marginTop: "0.3rem", paddingLeft: "1.1rem" }}>
                        <li>{translate("homepage.beforeYouStartList1")}</li>
                        <li>{translate("homepage.beforeYouStartList2")}</li>
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default Home;