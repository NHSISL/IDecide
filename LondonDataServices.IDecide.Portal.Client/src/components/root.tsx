import { Outlet, useLocation } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import LeftProgress from "./leftProgress/leftProgress";
import FooterComponent from "./layouts/footer";
import HeaderComponent from "./layouts/header";
import { useStep } from "../hooks/useStep";

export default function Root() {
    const location = useLocation();
    const cleanPath = location.pathname.replace(/\/+$/, '');

    const doNotShowLeftPanelRoutes = [
        "/home",
        "/end",
        "/copyright",
        "/about",
        "/contact",
        "/websitePrivacyNotice",
        "/cookieUse",
        "/accessibilityStatement"
    ];

    const doNotShowLeftPanel = doNotShowLeftPanelRoutes.includes(cleanPath);
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    return (
        <div className="root-layout">
            {!doNotShowLeftPanel && (
                <HeaderComponent />
            )}

            <div className="root-content">
                <Container fluid>
                    <Row>
                        {!doNotShowLeftPanel && (
                            <Col md={3}>
                                <LeftProgress
                                    currentStepIndex={currentStepIndex}
                                    setCurrentStepIndex={setCurrentStepIndex}
                                />
                            </Col>
                        )}
                        <Col md={doNotShowLeftPanel ? 12 : 9}>
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