import React from 'react'
import './styles/index.scss'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import App from './App';
import ReactDOM from 'react-dom/client'

// i18next setup
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

// Import translation JSON files using ES modules
import enTranslation from '../src/locales/en/translation.json';
import frTranslation from '../src/locales/fr/translation.json';
import roTranslation from '../src/locales/ro/translation.json';
import esTranslation from '../src/locales/es/translation.json';

i18n
    .use(initReactI18next)
    .init({
        resources: {
            en: { translation: enTranslation },
            fr: { translation: frTranslation },
            ro: { translation: roTranslation },
            es: { translation: esTranslation }
        },
        lng: 'en', // default language
        fallbackLng: 'en',
        interpolation: { escapeValue: false }
    });

const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);

root.render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
);