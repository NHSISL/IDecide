import { Outlet } from "react-router-dom";
import { Container } from "react-bootstrap";
import React from 'react';
import { Header } from "nhsuk-react-components";

export default function Root() {
    return (
        <>
            <Header>
                <Header.Container>
                    <Header.Logo href="/" />
                    <Header.Content>
                       
                    </Header.Content>
                </Header.Container>
                <Header.Nav>
                </Header.Nav>
            </Header>

            {/* Optional Header/Nav for all pages */}
            <Container fluid className="p-3">
                <Outlet />
            </Container>
        </>
    );
}
