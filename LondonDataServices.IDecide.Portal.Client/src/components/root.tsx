import { Outlet, useLocation } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import React from 'react';
import { Footer, Header } from "nhsuk-react-components";
import LeftProgress from "./leftProgress/leftProgress";
import { useStep } from "./context/stepContext";

export default function Root() {
    const location = useLocation();

    const cleanPath = location.pathname.replace(/\/+$/, '');
    const doNotShowLeftPanelRoutes = ["/home", "/end"];
    const doNotShowLeftPanel = doNotShowLeftPanelRoutes.includes(cleanPath);
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    return (
        <div className="root-layout">
            {!doNotShowLeftPanel && (
                <Header>
                    <Header.Container>
                        <Header.Logo href="/" />
                        <Header.Content>
                            {/* Optional content */}
                        </Header.Content>
                    </Header.Container>
                    <Header.Nav>
                        {/* Optional nav */}
                    </Header.Nav>
                </Header>
            )}

            <div className="root-content">
                <Container>
                    <Row>
                        {!doNotShowLeftPanel && (
                            <Col md={5}>
                                <LeftProgress currentStepIndex={currentStepIndex} setCurrentStepIndex={setCurrentStepIndex} />
                            </Col>
                        )}
                        <Col md={doNotShowLeftPanel ? 12 : 7}>
                            <Outlet />
                        </Col>
                    </Row>
                </Container>
            </div>

            <Footer />
        </div>
    );
}