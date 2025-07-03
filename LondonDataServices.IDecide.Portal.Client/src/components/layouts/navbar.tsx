import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { Button, Container, Navbar } from "react-bootstrap";
import Login from '../securitys/login';
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

interface NavbarComponentProps {
    toggleSidebar: () => void;
    showMenuButton: boolean;
}

const NavbarComponent: React.FC<NavbarComponentProps> = ({ toggleSidebar, showMenuButton }) => {

    const { configuration } = useFrontendConfiguration()

    return (
        <Navbar sticky="top" style={{ backgroundColor: configuration?.bannerColour || "#f8f9fa" }}>
            <Container fluid>
                {showMenuButton && (
                    <Button onClick={toggleSidebar} variant="outline-light" className="ms-3">
                        <FontAwesomeIcon icon={faBars} />
                    </Button>
                )}
                <Navbar.Brand href="/" className="me-auto ms-3 d-flex align-items-center">
                    <img src="/OneLondon_Logo_OneLondon_Logo_White.png" alt="London Data Service logo" height="35" width="108" />
                    <span className="d-none d-md-inline text-white" style={{ marginLeft: "10px" }}>
                        Re-Identification Portal
                        <small> {configuration?.environment !== "Live" && <>&nbsp;({configuration?.environment})</>}</small>
                    </span>
                </Navbar.Brand>
                <Navbar.Text>
                    <Login />
                </Navbar.Text>
            </Container>
        </Navbar>
    );
}

export default NavbarComponent;