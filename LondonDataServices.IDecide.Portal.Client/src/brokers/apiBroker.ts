import axios from 'axios';
import { InteractionRequiredAuthError, PopupRequest, PublicClientApplication, RedirectRequest } from "@azure/msal-browser";
import { MsalConfig } from '../authConfig';

class ApiBroker {

    loginRequest?: PopupRequest;
    msalInstance? : PublicClientApplication;

    private async initialize() {
      //  const configuration = await MsalConfig.build();
        this.loginRequest = MsalConfig.loginRequest;
        this.msalInstance = new PublicClientApplication(MsalConfig.msalConfig);
        await this.msalInstance.initialize();
    }

    private async acquireAccessToken() {
        await this.initialize(); // Ensure MSAL is initialized

        if (!this.msalInstance) {
            throw "msalInstance not correctly initialised";
        }

        if (!this.loginRequest) {
            throw "login request not correctly initialised";
        }

        const activeAccount = this.msalInstance.getActiveAccount();
        const accounts = this.msalInstance.getAllAccounts();

        const request = {
            scopes: this.loginRequest.scopes,
            account: activeAccount || accounts[0]
        };

        let authResult;
        try {
            authResult = await this.msalInstance.acquireTokenSilent(request);
        } catch (error) {
            if (error instanceof InteractionRequiredAuthError) {
                // fallback to interaction when silent call fails    
                const redirectRequest: RedirectRequest = {
                    ...request,
                    redirectUri: window.location.href
                };
                await this.msalInstance.acquireTokenRedirect(redirectRequest);
            } else {
                console.log(error);
                throw error; // rethrow the error after logging it
            }
        }
        return authResult ? authResult.accessToken : null;
    }

    private async config() {
        const accessToken = await this.acquireAccessToken();
        if (accessToken) {
            return { headers: { 'Authorization': 'Bearer ' + accessToken } }
        }

        return {};
    }

    public async GetAsync(queryFragment: string) {
        const url = queryFragment;
        return axios.get(url, await this.config());
    }

    //public async GetAsyncAbsolute(absoluteUri: string) {
    //    return axios.get(absoluteUri, await this.config());
    //}

    public async GetAsyncAbsolute(absoluteUri: string) {
        return axios.get(absoluteUri, {
            headers: {
                'Cache-Control': 'no-cache',
                'Pragma': 'no-cache',
                'Expires': '0'
            }
        });
    }

    public async PostAsync(relativeUrl: string, data: unknown) {
        const url = relativeUrl;

        return axios.post(url,
            data,
            await this.config()
        );
    }

    public async PostFormAsync(relativeUrl: string, data: FormData) {
        const url = relativeUrl;

        const headers = {
            'Authorization': 'Bearer ' + await this.acquireAccessToken(),
            "Content-Type": 'multipart/form-data'
        }

        return axios.post(url,
            data,
            { headers }
        );
    }

    public async PutAsync(relativeUrl: string, data: unknown) {
        const url = relativeUrl;

        return axios.put(url, data, await this.config());
    }

    public async DeleteAsync(relativeUrl: string) {
        const url = relativeUrl;

        return axios.delete(url, await this.config());
    }
}

export default ApiBroker;