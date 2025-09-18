export class PowerOfAttorney {
    public firstName?: string;
    public surname?: string;
    public relationship?: string;

    constructor(powerOfAttorney: PowerOfAttorney) {
        this.firstName = powerOfAttorney.firstName || "";
        this.surname = powerOfAttorney.surname || "";
        this.relationship = powerOfAttorney.relationship || "";
    }
}