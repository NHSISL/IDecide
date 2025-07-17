import React, { useState } from "react";
import { Button } from "nhsuk-react-components";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlusCircle, faMinusCircle } from "@fortawesome/free-solid-svg-icons";

const expandableHeaders = [
    "What is the London Data Service",
    "What is the process for opting out",
    "What can i opt out of on the portal",
    "What does the London Date Service Enable",
    "The London Date Service Privacy Notice"
];

const expandableContent = [
    "The London Data Service is ...", // Add your content here
    "The process for opting out is ...",
    "You can opt out of ...",
    "The London Data Service enables ...",
    "Privacy Notice: ..."
];

const ExpandableSection = ({ header, content, expanded, onClick }: {
    header: string;
    content: string;
    expanded: boolean;
    onClick: () => void;
}) => (
    <div className="expandable-section">
        <button
            type="button"
            onClick={onClick}
            style={{
                background: "none",
                border: "none",
                width: "100%",
                textAlign: "left",
                fontWeight: "bold",
                fontSize: "1rem",
                padding: "0.5rem 0",
                cursor: "pointer",
                display: "flex",
                alignItems: "center"
            }}
            aria-expanded={expanded}
        >
            <span style={{ marginRight: "0.75rem" }}>
                <FontAwesomeIcon
                    icon={expanded ? faMinusCircle : faPlusCircle}
                    color="black"
                    style={{ fontSize: "28px" }}
                />
            </span>
            {header}
        </button>
        {expanded && (
            <div style={{ padding: "0.5rem 0 1rem 2.25rem" }}>
                {content}
            </div>
        )}
    </div>
);

export const HomePage = () => {
    const navigate = useNavigate();
    const [expandedIndex, setExpandedIndex] = useState<number | null>(null);

    return (
        <div className="fullscreen-bg">
            <div className="fullscreen-bg-overlay">
                <div className="home-box">
                    <h1>Welcome to the London Data Portal</h1>
                    <p>Manage your data choices simply and securely</p>
                    {expandableHeaders.map((header, idx) => (
                        <ExpandableSection
                            key={header}
                            header={header}
                            content={expandableContent[idx]}
                            expanded={expandedIndex === idx}
                            onClick={() => setExpandedIndex(expandedIndex === idx ? null : idx)}
                        />
                    ))}
                    <Button onClick={() => navigate("/optOut")}>Start</Button>
                </div>
            </div>
        </div>
    );
};