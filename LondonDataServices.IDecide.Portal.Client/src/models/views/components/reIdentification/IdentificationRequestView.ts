import { IdentificationItemView } from "./IdentificationItemView";

export class IdentificationRequestView {
    public id: string = "";
    public identificationItems: IdentificationItemView[] = [];
    public UserId: string = "";
    public givenName: string = "";
    public surname: string = "";
    public displayName: string = "";
    public jobTitle: string = "";
    public email: string = "";
    public purpose: string = "";
    public organisation: string= "";
    public reason: string= "";
}
