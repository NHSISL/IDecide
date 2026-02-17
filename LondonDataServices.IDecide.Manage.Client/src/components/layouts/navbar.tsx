import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { Button, Container, Navbar } from "react-bootstrap";

interface NavbarComponentProps {
    toggleSidebar: () => void;
    showMenuButton: boolean;
}

const logout = async () => {
    try {
        await fetch('/auth/logout', {
            method: "POST",
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        window.location.href = '/';
    } catch (error) {
        console.error('Logout failed:', error);
        window.location.href = '/';
    }
}

const NavbarComponent: React.FC<NavbarComponentProps> = ({ toggleSidebar, showMenuButton }) => {

    return (
        <Navbar className="bg-light" sticky="top">
            <Container fluid>
                {showMenuButton && (
                    <Button onClick={toggleSidebar} variant="outline-dark" className="ms-3">
                        <FontAwesomeIcon icon={faBars} />
                    </Button>
                )}
                <Navbar.Brand href="/" className="me-auto ms-3 d-flex align-items-center">
                    <img src="/OneLondon_Logo_OneLondon_Logo_Blue.png" alt="London Data Service logo" height="35" width="108" />
                    <span className="d-none d-md-inline" style={{ marginLeft: "10px" }}>
                        Opt-Out Management Portal
                    </span>
                </Navbar.Brand>
                <Navbar.Text>

                    <Button onClick={logout}>Logout</Button>
                </Navbar.Text>
            </Container>
        </Navbar>
    );
}

export default NavbarComponent;