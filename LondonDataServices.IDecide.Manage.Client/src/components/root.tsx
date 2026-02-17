import { Outlet } from "react-router-dom";
import NavbarComponent from "./layouts/navbar";
import { useState, useEffect } from "react";
import SideBarComponent from "./layouts/sidebar";
import FooterComponent from "./layouts/footer";
import LoginUnAuthorisedComponent from "./layouts/loginUnauth";

export default function Root() {
    const [sidebarOpen, setSidebarOpen] = useState(true);

    const toggleSidebar = () => {
        setSidebarOpen(!sidebarOpen);
    };

    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        fetch('/auth/session')
            .then(async (r) => {
                if (r.status === 200) {
                    const data = await r.json();
                    console.log('sub:', data.sub);
                    console.log('upn:', data.upn);
                    console.log('name:', data.name);
                    console.log('roles:', data.roles);
                    console.log('expiry:', data.expiresAt);
                    setIsAuthenticated(true);
                } else {
                    setIsAuthenticated(false);
                }
            });
    }, [])

    useEffect(() => {
        const handleResize = () => {
            if (window.innerWidth < 1024) {
                setSidebarOpen(false);
            } else {
                setSidebarOpen(true);
            }
        };

        window.addEventListener('resize', handleResize);

        handleResize();

        return () => {
            window.removeEventListener('resize', handleResize);
        };
    }, []);

    if (!isAuthenticated) {
        return (<LoginUnAuthorisedComponent />);
    }

    return (
        <>
            <div className="layout-container">
                <div className={`sidebar bg-light ${sidebarOpen ? 'sidebar-open' : 'sidebar-closed'}`}>
                    <SideBarComponent />
                    <div className="footerContent">
                        <FooterComponent />
                    </div>
                </div>

                <div className={`content ${sidebarOpen ? 'content-shift-right' : 'content-shift-left'}`}>
                    <NavbarComponent toggleSidebar={toggleSidebar} showMenuButton={true} />

                    <div className="content-inner">
                        <Outlet />
                    </div>

                </div>
            </div>
        </>
    );
}