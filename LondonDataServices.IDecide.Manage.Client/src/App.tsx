/* eslint-disable @typescript-eslint/no-explicit-any */
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { MsalProvider } from '@azure/msal-react';
import { SecuredRoute } from './components/securitys/securedRoutes';
import securityPoints from './securityMatrix';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClientGlobalOptions } from './brokers/apiBroker.globals';
import { Home } from './pages/home';
import { ConfigurationHome } from './pages/configuration/configurationHome';
import { Lookups } from './pages/configuration/lookups';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { UserAccessNew } from './components/userAccess/userAccessNew';
import "react-toastify/dist/ReactToastify.css";
import ToastBroker from './brokers/toastBroker';
import { UserAccessEdit } from './components/userAccess/userAccessEdit';
import { CsvIdentificationRequestPage } from './pages/csvIdentificationRequest';
import { CsvIdentificationRequestDetailPage } from './pages/csvIdentificationRequestDetail';
import { ImpersonationContext } from './pages/impersonationContext';
import { ImpersonationContextDetailPage } from './pages/impersonationContextDetail';
import { OdsData } from './pages/odsData';
import { PdsData } from './pages/pdsData';
import { AccessAuditPage } from './pages/accesssAuditPage';
import { UserAccessPage } from './pages/userAccessPage';

function App({ instance }: any) {

    const router = createBrowserRouter([
        {
            path: "/",
            element: <Root />,
            errorElement: <ErrorPage />,
            children: [
                {
                    path: "home",
                    element: <Home />
                },
                {
                    path: "configuration/home",
                    element: <SecuredRoute allowedRoles={securityPoints.configuration.view}><ConfigurationHome /></SecuredRoute> 
                },
                {
                    path: "configuration/lookups",
                    element: <SecuredRoute allowedRoles={securityPoints.configuration.view}><Lookups /></SecuredRoute>
                },
                {
                    path: "userAccess",
                    element: <SecuredRoute allowedRoles={securityPoints.userAccess.view}><UserAccessPage /></SecuredRoute>
                },
                {
                    path: "userAccess/newUser",
                    element: <SecuredRoute allowedRoles={securityPoints.userAccess.add}><UserAccessNew /></SecuredRoute>
                },
                {
                    path: "userAccess/:UserId",
                    element: <SecuredRoute allowedRoles={securityPoints.userAccess.add}><UserAccessEdit/></SecuredRoute>
                },
                {
                    path: "impersonationContext",
                    element: <ImpersonationContext />
                },
                {
                    path: "impersonationContextDetail/:ImpersonationContextId",
                    element: <ImpersonationContextDetailPage />
                },
                {
                    path: "csvIdentificationRequest",
                    element: <CsvIdentificationRequestPage />
                },
                {
                    path: "csvIdentificationRequestDetail/:CsvIdentificationRequestId",
                    element: <CsvIdentificationRequestDetailPage />
                },
                {
                    path: "odsData",
                    element: <OdsData />
                },
                {
                    path: "pdsData",
                    element: <PdsData />
                },
                {
                    path: "accessAudit",
                    element: <AccessAuditPage />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                }
            ]
        }
    ]);

    return (
        <>
            <MsalProvider instance={instance}>
                <QueryClientProvider client={queryClientGlobalOptions}>
                    <RouterProvider router={router} />
                    <ReactQueryDevtools initialIsOpen={false} />
                </QueryClientProvider>
                <ToastBroker.Container />
            </MsalProvider>
        </>
    );


}

export default App;