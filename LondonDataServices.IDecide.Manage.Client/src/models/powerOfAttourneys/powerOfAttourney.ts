export class PowerOfAttourney {
    public firstName?: string;
    public surname?: string;
    public relationship?: string;

    constructor(powerOfAttourney: PowerOfAttourney) {
        this.firstName = powerOfAttourney.firstName || "";
        this.surname = powerOfAttourney.surname || "";
        this.relationship = powerOfAttourney.relationship || "";
    }
}