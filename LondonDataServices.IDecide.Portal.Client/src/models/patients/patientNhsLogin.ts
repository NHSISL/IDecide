export class PatientNhsLogin {
    nhsNumber: string;
    givenName: string;
    surname: string;
    dateOfBirth?: Date;
    email?: string;
    phone?: string;

    constructor(data: {
        nhsNumber: string;
        givenName: string;
        surname: string;
        dateOfBirth?: Date | string;
        email?: string;
        phone?: string;
    }) {
        this.nhsNumber = data.nhsNumber;
        this.givenName = data.givenName;
        this.surname = data.surname;
        this.dateOfBirth = data.dateOfBirth
            ? new Date(data.dateOfBirth)
            : undefined;
        this.email = data.email;
        this.phone = data.phone;
    }
}