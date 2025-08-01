import React, { useState } from "react";
import { Button } from "nhsuk-react-components";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlusCircle, faMinusCircle } from "@fortawesome/free-solid-svg-icons";

const expandableHeaders = [
    "What is the London Data Service?",
    "How will the London Data Service and the London Analytics Platform use my data?",
    "How do I stop LDS & SDE using my data?",
    "Where can I see the privacy Notices for LDS & SDE?"
];

const expandableContent: React.ReactNode[][] = [
    [
        <p key="p1">
            The London Data Service securely collects patient information from a range of healthcare locations around London such as GP surgeries and hospitals.
            It then organises and stores this information ready to be used by other approved systems.
        </p>,
        <p key="p2">
            This securely stored data about patients using London's healthcare services can be distributed to analytics and care platforms throughout London such as the
            London Care Record or the London Analytics Platform.
        </p>
    ],
    [
        <p key="p1">
            Both systems will only share identifiable data - data which can be used to tell someone who you are - with services that use your data for
            directly providing healthcare services to you.
        </p>,
        <p key="p2">
            Both systems provide data to 'secondary' services that use patient data for healthcare planning & design, population health management and healthcare research.
            This information is incredibly useful for providers across London to understand healthcare needs of Londoners. It helps us in designing NHS services,
            forecast demand and understand local neighbourhood needs compared to London as a whole. Research projects into particular conditions including how
            illnesses progress and possible treatments is another important service that use this data and can benefit Londoners and the health of the UK as a whole.
            These secondary services will only ever be sent de-identified data - data that does not include information which could tell someone who you are
            (such as names, addresses and dates of birth).
        </p>
    ],
    [
        <p key="p1">
            Using this portal you can tell us that you don't want your data used for secondary purposes, such as population health planning and research.
            To register your details to 'opt-out' click the start button below.
        </p>,
        <p key="p2">
            Telling us that you don't want your data shared with healthcare professionals who will be treating you is a different process.
            To do that please e-mail <a href="mailto:NELondonicb.oneLondon.opt-out@nhs.net">NELondonicb.oneLondon.opt-out@nhs.net</a> and make the subject of the e-mail "Dissent from data sharing for direct care".
        </p>
    ],
    [
        <p key="p3">
            You can access our privacy notices on this page of the portal [Privacy notice URL]
        </p>
    ]
];

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
    const navigate = useNavigate();
    const [expandedIndex, setExpandedIndex] = useState<number | null>(null);

    return (
        <div className="home-content" style={{ padding: "1.5rem 0.5rem" }}>
            <div className="home-box" style={{ maxWidth: 1700, margin: "0 auto", background: "#fff", borderRadius: 8, boxShadow: "0 2px 8px #e0e0e0", padding: "2rem 1.5rem" }}>
                <h1 style={{ fontSize: "1.7rem", marginBottom: "0.7rem" }}>Welcome to the OneLondon Data Portal</h1>
                <p style={{ marginBottom: "0.5rem" }}>OneLondon have developed a world leading resource for health and care improvement known as the London SDE (Secure Data Environment). This is comprised of the LDS (London Data Service) and the LAP (London Analytics Platform).</p>
                <p style={{ marginBottom: "0.5rem" }}>The data collected from healthcare systems across London by these services can be used for many things all ensuring patient information is shared for improved provision of care. </p>
                <p style={{ marginBottom: "0.5rem" }}>This portal is where you can tell us if you don't want your data used by LDS or LAP for anything other than your personal healthcare. By clicking the start button below you will be taken through a process to exercise your right to request that we do not use your data for anything other than care provided to you by healthcare professionals. If you ever want to change that decision in the future you can go through the same process to tell us we can use your data. </p>
                <p style={{ marginBottom: "0.7rem" }}>
                    <strong>Click on the Start button below to tell us your data preference</strong>
                </p>
                <Button
                    onClick={() => navigate("/optOut")}
                    style={{ margin: "0 0 1rem 1rem", width: 160, fontWeight: 600, minHeight: 75 }}>
                    Start
                </Button>

                <Button
                    onClick={() => navigate("/optOut", { state: { powerOfAttourney: true } })}
                    style={{ margin: "0 0 1rem 1rem", width: 260, fontWeight: 600, minHeight: 75 }}>
                    Requesting an Opt-out on someone else's behalf
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
                    <strong>Before you start</strong>
                    <ul style={{ marginTop: "0.3rem", paddingLeft: "1.1rem" }}>
                        <li>
                            You'll need your 10-digit NHS Number or your Full name, Postcode &amp; Date Of Birth so that we can identify you.
                        </li>
                        <li>
                            We will be sending an e-mail, SMS text message or letter to the contact details you have registered with your GP. This will help us confirm we are speaking with the right person. You should be confident that your GP has your up-to-date contact details.
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default Home;