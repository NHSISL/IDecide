import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { HomePage } from './pages/homePage';
import { StepProvider } from './components/context/stepContext';
import { PositiveConfirmationPage } from './pages/positiveConfirmationPage';
import { AppFlowPage } from './pages/appFlowPage';
import CopyWritePage  from './pages/helpPages/copyrightPage';
import WebsitePrivacyNoticePage from './pages/helpPages/websitePrivacyNoticePage';
import AboutPage from './pages/helpPages/aboutPage';
import AccessibilityStatementPage from './pages/helpPages/accessibilityStatementPage';
import CookieUsePage from './pages/helpPages/cookieUsePage';
import ContactPage from './pages/helpPages/contactPage';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClientGlobalOptions } from './brokers/apiBroker.globals';
import SearchByNhsNumber from './components/SearchNhsNumber/searchByNhsNumber';
import NhsLoginOptOutPage from './pages/nhsLoginOptOutPage';

function App() {

    const router = createBrowserRouter([
        {
            path: "/",
            element: (
                <StepProvider>
                    <Root />
                </StepProvider>
            ),
            errorElement: <ErrorPage />,
            children: [
                {
                    path: "home",
                    element: <HomePage />
                },
                {
                    path: "optOut",
                    element: <AppFlowPage />
                },
                {
                    path: "copyright",
                    element: <CopyWritePage />
                },
                {
                    path: "about",
                    element: <AboutPage />
                },
                {
                    path: "contact",
                    element: <ContactPage />
                },
                {
                    path: "websitePrivacyNotice",
                    element: <WebsitePrivacyNoticePage />
                },
                {
                    path: "accessibilityStatement",
                    element: <AccessibilityStatementPage />
                },
                {
                    path: "cookieUse",
                    element: <CookieUsePage />
                },
                {
                    path: "positiveConfirmation",
                    element: <PositiveConfirmationPage />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                },
                {
                    path: "test-poa",
                    element: <SearchByNhsNumber onIDontKnow={() => { }} powerOfAttorney={true} />
                },
                {
                    path: "nhs-optOut",
                    element: <NhsLoginOptOutPage />
                }

            ]
        },
    ]);

    return (
        <>
            <QueryClientProvider client={queryClientGlobalOptions}>
                <RouterProvider router={router} />
            </QueryClientProvider>
        </>
    );


}

export default App;