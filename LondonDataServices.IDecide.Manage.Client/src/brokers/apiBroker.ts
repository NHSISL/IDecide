import axios from 'axios';

class ApiBroker {

    public async GetAsync(queryFragment: string) {
        const url = queryFragment;
        return axios.get(url, { withCredentials: true });
    }

    public async GetAsyncUnauthenticated(queryFragment: string) {
        const url = queryFragment;

        return axios.get(url, { withCredentials: true });
    }

    public async GetAsyncAbsolute(absoluteUri: string) {
        return axios.get(absoluteUri, { withCredentials: true });
    }

    public async PostAsync(relativeUrl: string, data: unknown) {
        const url = relativeUrl;

        return axios.post(url, data, { withCredentials: true });
    }

    public async PostFormAsync(relativeUrl: string, data: FormData) {
        const url = relativeUrl;

        const config = {
            headers: {
                "Content-Type": 'multipart/form-data'
            },
            withCredentials: true
        };

        return axios.post(url, data, config);
    }

    public async PutAsync(relativeUrl: string, data: unknown) {
        const url = relativeUrl;

        return axios.put(url, data, { withCredentials: true });
    }

    public async DeleteAsync(relativeUrl: string) {
        const url = relativeUrl;

        return axios.delete(url, { withCredentials: true });
    }
}

export default ApiBroker;