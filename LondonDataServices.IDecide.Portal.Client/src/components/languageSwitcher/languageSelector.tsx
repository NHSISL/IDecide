import React from 'react';
import { useTranslation } from 'react-i18next';

const LanguageSelector: React.FC = () => {
    const { i18n } = useTranslation();

    const handleChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        i18n.changeLanguage(event.target.value);
    };

    return (
        <select
            value={i18n.language}
            onChange={handleChange}
            className="form-select"
            style={{ width: 120 }}
        >
            <option value="en" > English </option>
            <option value="fr" > French </option>
            <option value="ro" > Romanian </option>
        </select>
    );
};

export default LanguageSelector;