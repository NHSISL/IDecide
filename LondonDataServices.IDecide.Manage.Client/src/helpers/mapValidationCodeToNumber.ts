export function mapValidationCodeToNumber(validationCode?: string | null): number | null {
    switch (validationCode?.toLowerCase()) {
        case "email":
            return 1;
        case "sms":
            return 2;
        case "letter":
            return 3;
        default:
            return null;
    }
}