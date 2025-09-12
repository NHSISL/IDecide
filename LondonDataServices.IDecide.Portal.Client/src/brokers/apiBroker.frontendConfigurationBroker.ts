import axios from "axios";

export type FrontendConfigurationResponse = {
    clientId: string,
    authority: string,
    scopes: string,
    environment: string,
    application: string,
    version: string,
    bannerColour: string,
    recaptchaSiteKey: string,
    heldeskContactEmail: string,
    heldeskContactNumber: string,
}

export type FrontendConfiguration = {
    clientId: string,
    authority: string,
    scopes: string[],
    environment: string,
    application: string,
    version: string,
    bannerColour: string,
    recaptchaSiteKey: string,
    heldeskContactEmail: string
    heldeskContactNumber: string
}

class FrontendConfigurationBroker {
    relativeFrontendConfigurationUrl = '/api/FrontendConfigurations/';

    async GetFrontendConfigruationAsync(): Promise<FrontendConfiguration> {
        const url = `${this.relativeFrontendConfigurationUrl}`;

        try {
            const response = (await axios.get<FrontendConfigurationResponse>(url)).data;

            const result: FrontendConfiguration = {
                ...response,
                scopes: response.scopes.split(',')
            }

            if (!result.clientId) {
                throw new Error("ClientId not provided");
            }

            if (!result.authority) {
                throw new Error("Authority not provided");
            }

            if (!result.scopes.length) {
                throw new Error("Scopes not provided");
            }

            if (!result.recaptchaSiteKey.length) {
                throw new Error("recaptchaSiteKey not provided");
            }

            if (!result.heldeskContactEmail.length) {
                throw new Error("heldesk contact email not provided");
            }

            if (!result.heldeskContactNumber.length) {
                throw new Error("heldesk contact number not provided");
            }

            return result;
        } catch (error) {
            console.error("Error fetching configuration", error);
            throw error;
        }
    }
}

export default FrontendConfigurationBroker;