import React, { useEffect } from "react";
import { Header, } from "nhsuk-react-components";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import AccessibilityBox from "../accessibilitys/accessibility";
import { useTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";
import { Button } from "react-bootstrap";

const HeaderComponent: React.FC = () => {
    const { configuration } = useFrontendConfiguration();
    const { t: translate } = useTranslation();
    const location = useLocation();

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
                            OneLondon Data Opt-Out Portal
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
                                alignItems: "center"
                            }}
                        >
                            {location.pathname === "/nhs-optOut" && (
                                <Button
                                    className="nhsuk-button--small"
                                    onClick={() => {
                                        fetch('/logout', { method: 'POST' }).then(d => {
                                            if (d.ok) {
                                                window.location.href = '/';
                                            }
                                        });
                                    }}
                                >
                                    Log out
                                </Button>
                            )}
                        </div>
                    </Header.Container>
                </Header>
            </div>
        </>
    );
};

export default HeaderComponent;