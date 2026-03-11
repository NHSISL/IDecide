import React, { useEffect } from "react";
import { Header } from "nhsuk-react-components";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import AccessibilityBox from "../accessibilitys/accessibility";
import { useTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";

const linkStyle: React.CSSProperties = {
    color: "#fff",
    textDecoration: "none",
    background: "none",
    boxShadow: "none"
};

const HeaderComponent: React.FC = () => {
    const { configuration } = useFrontendConfiguration();
    const { t: translate } = useTranslation();
    const location = useLocation();

    const showAccountActions =
        location.pathname === "/nhs-optOut" ||
        location.pathname === "/optOut" ||
        location.pathname === "/nhsLoginHome";

    useEffect(() => {
        if (configuration?.bannerColour) {
            document.documentElement.style.setProperty('--nhsuk-header-bg', configuration.bannerColour);
        }
    }, [configuration?.bannerColour]);

    const handleLogout = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        fetch('/logout', { method: 'POST' }).then(d => {
            if (d.ok) {
                window.location.href = '/';
            }
        });
    };

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
                    <div className="test-env-banner" >
                        <small style={{ color: "#fff", fontWeight: 600, letterSpacing: "0.02em" }}>
                            <strong style={{ color: "#fff", textTransform: "uppercase" }}>
                                {configuration?.environment}:
                            </strong>
                            &nbsp;{translate("DevEnvironmentWarning.bannerMessage")}
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
                <Header>
                    <Header.Container>
                        <Header.Logo href="/home" />
                        <Header.ServiceName href="/home">
                            London Secure Data Environment Data Portal
                        </Header.ServiceName>
                        <Header.Content>
                            <div style={{ display: "flex", alignItems: "center", gap: "1.5rem" }}>
                                <AccessibilityBox />
                            </div>
                        </Header.Content>
                        <div
                            className="logout-header"
                            style={{
                                marginLeft: "auto",
                                display: "flex",
                                alignItems: "center",
                                gap: "0.75rem"
                            }}
                        >
                            {showAccountActions && configuration?.manageNhsDetailsUri && (
                                <>
                                    <a
                                        href={configuration?.manageNhsDetailsUri}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        style={linkStyle}
                                        className="header-link"
                                    >
                                        Manage NHS account
                                    </a>
                                    <span style={{ color: "#fff", margin: "0 0.5rem" }}>|</span>
                                    <a
                                        href="/logout"
                                        onClick={handleLogout}
                                        style={linkStyle}
                                        className="header-link"
                                    >
                                        Log out
                                    </a>
                                </>
                            )}
                        </div>
                    </Header.Container>
                </Header>
            </div>
        </>
    );
};

export default HeaderComponent;