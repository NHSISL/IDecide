/* eslint-disable @typescript-eslint/no-explicit-any */
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { Home } from './pages/home';

function App() {

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