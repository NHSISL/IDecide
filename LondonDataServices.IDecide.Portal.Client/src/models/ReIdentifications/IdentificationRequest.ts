import { IdentificationItem } from "./IdentificationItem";

export class IdentificationRequest {
    public id: string = crypto.randomUUID();
    public identificationItems: IdentificationItem[] = [];
    public displayName: string = "";
    public surname?: string = "";
    public givenName?: string = "";
    public email: string = "";
    public jobTitle?: string = "";
    public organisation?: string = "";
    public reason: string = "";
}
