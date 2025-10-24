import { useCallback } from "react";

export function useNhsNumberValidator() {
    const validate = useCallback((nhsNumber: string): boolean => {
        const clean = nhsNumber.replace(/\s+/g, "");

        if (!/^\d{10}$/.test(clean)) return false;

        const digits = clean.split("").map(Number);
        const checkDigit = digits[9];
        const sum = digits
            .slice(0, 9)
            .reduce((acc, digit, i) => acc + digit * (10 - i), 0);

        const remainder = sum % 11;
        let expected = 11 - remainder;
        if (expected === 11) expected = 0;

        if (expected === 10 || expected !== checkDigit) return false;

        return true;
    }, []);

    return { validate };
}
