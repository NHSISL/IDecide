import React from 'react'
import './index.scss'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import App from './App';
import ReactDOM from 'react-dom/client'

const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);


root.render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
);

