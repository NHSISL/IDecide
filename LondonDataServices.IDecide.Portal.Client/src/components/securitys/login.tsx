import React from 'react';
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { Button, NavDropdown } from "react-bootstrap";
import { UserProfile } from '../securitys/userProfile';
import { MsalConfig } from '../../authConfig';

const Login: React.FC = () => {
    const { instance } = useMsal();
    const activeAccount = instance.getActiveAccount();

    const handleLogoutRedirect = () => {
        instance.logout().catch((error) => console.log(error));
    };

    const handleLoginRedirect = async () => {
        const loginRequest = MsalConfig.loginRequest;
        instance.loginRedirect(loginRequest).catch((error) => console.log(error));
    };

    return (
        <>
            <UnauthenticatedTemplate>
                <div className="collapse navbar-collapse justify-content-end">
                    <Button onClick={handleLoginRedirect} className="me-3">Sign in</Button>
                </div>
            </UnauthenticatedTemplate>
            <AuthenticatedTemplate>
                <NavDropdown
                    title={<span style={{ color: 'white' }}>{activeAccount?.username}</span>}
                    id="collasible-nav-dropdown" 
                    className="me-3">
                    <NavDropdown.Item onClick={handleLogoutRedirect}>Sign out</NavDropdown.Item>
                    <UserProfile />
                </NavDropdown>
            </AuthenticatedTemplate>
        </>
    );
};

export default Login;