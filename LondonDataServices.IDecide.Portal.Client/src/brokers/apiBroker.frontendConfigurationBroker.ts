import axios from "axios";

export type FrontendConfigurationResponse = {
    clientId: string,
    authority: string,
    scopes: string,
    environment: string,
    application: string,
    version: string,
    bannerColour: string,
    activeAgreement: string,
    reportMaxReId: string,
    reportBreachThreshold: string,
    csvMaxReId: string,
    supportContactEmail: string,
    blobStoreBaseUrl: string
}

export type FrontendConfiguration = {
    clientId: string,
    authority: string,
    scopes: string[],
    environment: string,
    application: string,
    version: string,
    bannerColour: string,
    activeAgreement: string,
    reportMaxReId: number,
    reportBreechThreshold: number,
    csvMaxReId: number,
    supportContactEmail: string,
    blobStoreBaseUrl: string
}

class FrontendConfigurationBroker {
    relativeFrontendConfigurationUrl = '/api/FrontendConfigurations/';

    async GetFrontendConfigruationAsync(): Promise<FrontendConfiguration> {
        const url = `${this.relativeFrontendConfigurationUrl}`;

        try {
            const response = (await axios.get<FrontendConfigurationResponse>(url)).data;

            const result: FrontendConfiguration = {
                ...response,
                reportMaxReId: parseInt(response.reportMaxReId),
                reportBreechThreshold: parseInt(response.reportBreachThreshold),
                csvMaxReId: parseInt(response.csvMaxReId),
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

            if (!result.csvMaxReId) {
                throw new Error("App configuration missing for FrontendConfiguration - CsvMaxReId.");
            }

            if (!result.supportContactEmail) {
                throw new Error("App configuration missing for FrontendConfiguration - SupportContactEmail.");
            }

            if (!result.blobStoreBaseUrl) {
                throw new Error("Blob Store Base Url not provided");
            }

            return result;
        } catch (error) {
            console.error("Error fetching configuration", error);
            throw error;
        }
    }
}

export default FrontendConfigurationBroker;