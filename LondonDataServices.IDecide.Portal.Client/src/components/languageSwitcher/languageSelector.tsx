import React, { useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import ReactCountryFlag from 'react-country-flag';

const languages = [
    { code: 'en', label: 'English', countryCode: 'GB' },
    { code: 'fr', label: 'French', countryCode: 'FR' },
    { code: 'ro', label: 'Romanian', countryCode: 'RO' },
    { code: 'es', label: 'Spanish', countryCode: 'ES' } // Added Spanish
];

// List of valid country codes for flags
const validCountryCodes = new Set(['GB', 'FR', 'RO', 'ES']); // Added ES

const LanguageSelector: React.FC = () => {
    const { i18n } = useTranslation();
    const [open, setOpen] = useState(false);
    const buttonRef = useRef<HTMLButtonElement>(null);

    const selectedLang = languages.find(l => l.code === i18n.language) || languages[0];

    const handleSelect = (code: string) => {
        i18n.changeLanguage(code);
        setOpen(false);
        buttonRef.current?.focus();
    };

    return (
        <div style={{ position: 'relative', width: 140 }}>
            <button
                ref={buttonRef}
                aria-haspopup="listbox"
                aria-expanded={open}
                aria-label="Select language"
                // Removed className="form-select"
                style={{
                    width: '100%',
                    textAlign: 'left',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    background: '#fff',
                    border: '1px solid #ccc',
                    borderRadius: 4,
                    padding: '0.5rem'
                }}
                onClick={() => setOpen((prev) => !prev)}
            >
                <span style={{ display: 'flex', alignItems: 'center' }}>
                    {validCountryCodes.has(selectedLang.countryCode) && (
                        <ReactCountryFlag
                            countryCode={selectedLang.countryCode}
                            svg
                            style={{ width: '1.5em', height: '1.5em', marginRight: 8 }}
                            aria-label={selectedLang.label}
                        />
                    )}
                    {selectedLang.label}
                </span>
                <span aria-hidden="true" style={{ marginLeft: 8 }}>▼</span>
            </button>
            {open && (
                <ul
                    role="listbox"
                    aria-label="Language options"
                    style={{
                        position: 'absolute',
                        top: '100%',
                        left: 0,
                        width: '100%',
                        background: '#fff',
                        border: '1px solid #ccc',
                        borderRadius: 4,
                        zIndex: 10,
                        margin: 0,
                        padding: 0,
                        listStyle: 'none'
                    }}
                >
                    {languages.map(lang => (
                        <li key={lang.code} role="option" aria-selected={i18n.language === lang.code}>
                            <button
                                onClick={() => handleSelect(lang.code)}
                                style={{
                                    width: '100%',
                                    textAlign: 'left',
                                    padding: '0.5rem',
                                    background: i18n.language === lang.code ? '#e0e0e0' : '#fff',
                                    border: 'none',
                                    display: 'flex',
                                    alignItems: 'center',
                                    cursor: 'pointer'
                                }}
                                aria-label={lang.label}
                            >
                                <span style={{ display: 'flex', alignItems: 'center' }}>
                                    {validCountryCodes.has(lang.countryCode) && (
                                        <ReactCountryFlag
                                            countryCode={lang.countryCode}
                                            svg
                                            style={{ width: '1.5em', height: '1.5em', marginRight: 8 }}
                                            aria-label={lang.label}
                                        />
                                    )}
                                    {lang.label}
                                </span>
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default LanguageSelector;