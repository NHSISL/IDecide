import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClientGlobalOptions } from './brokers/apiBroker.globals';
import { Home } from './pages/home';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import "react-toastify/dist/ReactToastify.css";
import ToastBroker from './brokers/toastBroker';
import { PatientSearchPage } from './pages/patientSearchPage';
import SearchByNhsNumberPage from './pages/searchByNhsNumberPage';
import ConfirmDetailsPage from './pages/confirmDetailsPage';
import { SendCodePage } from './pages/sendCodePage';
import ConfirmCodePage from './pages/confirmCodePage';
import { OptInOutPage } from './pages/optInOutPage';
import { ConfirmationPage } from './pages/confirmationPage';
import { StepProvider } from './components/context/stepContext';
import { PatientDetailsSearchPage } from './pages/patientDetailsSearchPage';
import { ThankyouPage } from './pages/thankyouPage';
import { ConsumersPage } from './pages/ConsumersPage';
import UnauthorisedPage from './pages/unauthorisedPage';

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
                    element: <Home />
                },
                {
                    path: "patientSearch",
                    element: <PatientSearchPage />
                },
                {
                    path: "nhsNumberSearch",
                    element: <SearchByNhsNumberPage />
                },
                {
                    path: "patientDetailsSearch",
                    element: <PatientDetailsSearchPage />
                },
                {
                    path: "confirmDetails",
                    element: <ConfirmDetailsPage />
                },
                {
                    path: "confirmCode",
                    element: <ConfirmCodePage />
                },
                {
                    path: "sendCode",
                    element: <SendCodePage />
                },
                {
                    path: "optInOut",
                    element: <OptInOutPage />
                },
                {
                    path: "confirmation",
                    element: <ConfirmationPage />
                },
                {
                    path: "thankyou",
                    element: <ThankyouPage />
                },
                {
                    path: "consumers",
                    element: <ConsumersPage />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                },
            ]
        },
        {
            path: "/unauthorised",
            element: <UnauthorisedPage />
        }
    ]);

    return (
        <>
            <QueryClientProvider client={queryClientGlobalOptions}>
                <RouterProvider router={router} />
                <ReactQueryDevtools initialIsOpen={false} />
            </QueryClientProvider>
            <ToastBroker.Container />
        </>
    );
}

export default App;