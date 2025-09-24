export function mapValidationCodeToNumber(validationCode?: string | null): number | null {
    switch (validationCode?.toLowerCase()) {
        case "email":
            return 0;
        case "letter":
            return 1;
        case "sms":
            return 2;
        default:
            return null;
    }
}