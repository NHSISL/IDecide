import { Footer } from 'nhsuk-react-components';
import React from 'react';
import { Container } from "react-bootstrap";

const FooterComponent: React.FC = () => {
    return (
        <Container fluid className="footer-center" style={{ zIndex: '1' }}>
            <Footer>
                <Footer.List>
                    <Footer.ListItem href="/copyright/"> Copyright </Footer.ListItem>
                    <Footer.ListItem href="/about/">About Us</Footer.ListItem>
                    <Footer.ListItem href="/contact/">Contact us</Footer.ListItem>
                    <Footer.ListItem href="/websitePrivacyNotice/"> Website privacy notice </Footer.ListItem>
                    <Footer.ListItem href="/accessibilityStatement/">Accessibility statement</Footer.ListItem>
                    <Footer.ListItem href="/cookieUse/">Cookie use</Footer.ListItem>
                </Footer.List>
                <Footer.Copyright>
                    <span className="footer-logos align-items-center text-center">
                        <img
                            src="/Picture1.png"
                            alt="OneLondon Logo"
                            className="footer-logo"
                        />
                    </span>
                </Footer.Copyright>
            </Footer>
        </Container>
    );
}

export default FooterComponent;