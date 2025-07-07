/* eslint-disable @typescript-eslint/no-explicit-any */
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { HomePage } from './pages/homePage';
import { ConfirmNhsNumber } from './pages/confirmNhsNumberPage';
import { StepProvider } from './components/context/stepContext';
import { ConfirmDetailsPage } from './pages/confirmDetailsPage';
import { PositiveConfirmationPage } from './pages/positiveConfirmationPage';

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
                    element: <ConfirmNhsNumber />
                },
                {
                    path: "confirmDetails",
                    element: <ConfirmDetailsPage />
                },
                {
                    path: "positiveConfirmation",
                    element: <PositiveConfirmationPage />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                }

            ]
        },
    ]);

    return (
        <>
            <RouterProvider router={router} />
        </>
    );


}

export default App;