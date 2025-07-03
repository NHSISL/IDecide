import ApiBroker from "./apiBroker";
import { UserAgreement } from "../models/userAgreement/userAgreement";

class UserAgreementBroker {
    relativeUserAgreementUrl = '/api/userAgreements';

    private apiBroker: ApiBroker = new ApiBroker();

    async UserHasSignedAgreement(userId: string, userAgreementVersion?: string, agreementType?: string): Promise<boolean> {
        if (!userId || !userAgreementVersion) {
            return false;
        }

        const url = `${this.relativeUserAgreementUrl}?$filter=UserId eq '${userId}' and AgreementVersion eq '${userAgreementVersion}' and AgreementType eq '${agreementType}'`;

        return await this.apiBroker.GetAsync(url)
            .then(result => {
                if (result.data) {
                    return result.data.length > 0;
                }
                return false;
            })
    }

    async SignAgreement(userAgreement: UserAgreement) {
        const url = this.relativeUserAgreementUrl;
        return await this.apiBroker.PostAsync(url, userAgreement)
    }
}

export default UserAgreementBroker;