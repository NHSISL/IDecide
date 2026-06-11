export class SessionData {
    sub: string;
    upn: string;
    name: string;
    roles: string[];
    expiresAt: string;

    constructor(
        sub: string,
        upn: string,
        name: string,
        roles: string[],
        expiresAt: string
    ) {
        this.sub = sub;
        this.upn = upn;
        this.name = name;
        this.roles = roles;
        this.expiresAt = expiresAt;
    }
}