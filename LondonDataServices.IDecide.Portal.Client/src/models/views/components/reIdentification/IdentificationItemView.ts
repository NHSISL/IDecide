export class IdentificationItemView {
    public rowNumber: string;
    public identifier: string;
    public message: string;
    public hasAccess?: boolean;
    public isReidentified?: boolean;

    constructor(
        rowNumber: string,
        identifier: string,
        message: string,
        hasAccess?: boolean,
        isReidentified?: boolean,
    ) {
        this.rowNumber = rowNumber ? rowNumber : "";
        this.identifier = identifier || "";
        this.message = message || "";
        this.hasAccess = hasAccess || false;
        this.isReidentified = isReidentified || false;
    }
}