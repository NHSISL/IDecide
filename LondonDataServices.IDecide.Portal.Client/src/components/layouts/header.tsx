import React, { useEffect } from "react";
import { Header } from "nhsuk-react-components";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import AccessibilityBox from "../accessibilitys/accessibility";

const HeaderComponent: React.FC = () => {
    const { configuration } = useFrontendConfiguration();

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
                        background: "#222",
                        zIndex: 2000,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center"
                    }}
                >
                    <div className="test-env-banner">
                        <small style={{ color: "#fff", fontWeight: 600, letterSpacing: "0.02em" }}>
                            <strong style={{ color: "#fff", textDecoration: "underline" }}>Test Environment:</strong>
                            &nbsp;This website is for demonstration and testing purposes only. Any information you enter here will not be saved to real patient records or used for clinical care. Please do not enter real patient data.
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
                            <span className="me-4 text-white">
                                {configuration?.environment}
                            </span>
                            <AccessibilityBox />
                        </Header.Content>
                    </Header.Container>
                </Header>
            </div>
        </>
    );
};

export default HeaderComponent;