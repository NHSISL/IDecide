import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import './index.scss'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import App from './App';
import { EventType, PublicClientApplication, EventMessage, AuthenticationResult } from '@azure/msal-browser';
import { MsalConfig } from './authConfig';

// i18next setup
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

// Import translation JSON files using ES modules
import enTranslation from '../public/locales/en/translation.json';
import frTranslation from '../public/locales/fr/translation.json';
import roTranslation from '../public/locales/ro/translation.json';
import esTranslation from '../public/locales/es/translation.json';

/**
 * MSAL should be instantiated outside of the component tree to prevent it from being re-instantiated on re-renders.
 * For more, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md
 */

MsalConfig.build().then(() => {
    const msalInstance = new PublicClientApplication(MsalConfig.msalConfig);
    msalInstance.initialize().then(() => {
        // Account selection logic is app dependent. Adjust as needed for different use cases.
        const accounts = msalInstance.getAllAccounts();
        if (accounts.length > 0) {
            msalInstance.setActiveAccount(accounts[0]);
        }

        msalInstance.addEventCallback((event: EventMessage) => {
            if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
                const payload = event.payload as AuthenticationResult;
                const account = payload.account;
                msalInstance.setActiveAccount(account);
            }
        });

        const root = ReactDOM.createRoot(
            document.getElementById("root") as HTMLElement
        );

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



        root.render(
            <React.StrictMode>
                <App instance={msalInstance} />
            </React.StrictMode>
        );
    });
});
