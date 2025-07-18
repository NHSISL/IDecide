import { Footer } from 'nhsuk-react-components';
import React from 'react';
import { Container, Row, Col } from "react-bootstrap";
import { Link } from "react-router-dom";

const FooterComponent: React.FC = () => {
    return (
        <Container fluid className="footer-center py-3">
            <Footer>
                <Footer.List>
                    <Footer.ListItem>
                        <Link to="/copyright" style={{ textDecoration: "none", color: "inherit" }}>
                            Copyright
                        </Link>
                    </Footer.ListItem>
                    <Footer.ListItem>
                        <Link to="/about" style={{ textDecoration: "none", color: "inherit" }}>
                            About us
                        </Link>
                    </Footer.ListItem>
                    <Footer.ListItem>
                        <Link to="/contact" style={{ textDecoration: "none", color: "inherit" }}>
                            Contact us
                        </Link>
                    </Footer.ListItem>
                    <Footer.ListItem>
                        <Link to="/websitePrivacyNotice" style={{ textDecoration: "none", color: "inherit" }}>
                            Website privacy notice
                        </Link>
                    </Footer.ListItem>
                    <Footer.ListItem>
                        <Link to="/accessibilityStatement" style={{ textDecoration: "none", color: "inherit" }}>
                            Accessibility statement
                        </Link>
                    </Footer.ListItem>
                    <Footer.ListItem>
                        <Link to="/cookieUse" style={{ textDecoration: "none", color: "inherit" }}>
                            Cookie use
                        </Link>
                    </Footer.ListItem>
                </Footer.List>
                <Footer.Copyright>
                    <Row className="footer-logos align-items-center text-center">
                        <Col xs={12} md="auto">
                            <img
                                src="/OneLondon_Logo_OneLondon_Logo_White.png"
                                alt="OneLondon Logo"
                                className="footer-logo"
                            />
                        </Col>
                        <Col xs={12} md="auto">
                            <img
                                src="/National_Health_Service.png"
                                alt="NHS Logo"
                                className="footer-logo nhs-logo"
                            />
                        </Col>
                    </Row>
                </Footer.Copyright>
            </Footer>
        </Container>
    );
}

export default FooterComponent;