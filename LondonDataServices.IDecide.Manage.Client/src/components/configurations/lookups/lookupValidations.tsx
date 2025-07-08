import { Validation } from "../../../models/validations/validation";

export const lookupValidations: Array<Validation> = [
    {
        property: "name",
        friendlyName: "name",
        isRequired: true,
        minLength: 3,
        maxLength: 450,
    },
    {
        property: "value",
        friendlyName: "value",
        isRequired: true,
        minLength: 3,
        maxLength: 6000,
    }
]