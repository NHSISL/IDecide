import axios from 'axios';

class ApiBroker {
    public async GetAsync(queryFragment: string) {
        const url = queryFragment;
        return axios.get(url);
    }

    public async GetAsyncAbsolute(absoluteUri: string) {
        return axios.get(absoluteUri, {
            headers: {
                'Cache-Control': 'no-cache',
                'Pragma': 'no-cache',
                'Expires': '0'
            }
        });
    }

    public async PostAsync(relativeUrl: string, data: unknown, headers?: Record<string, string>) {
        const url = relativeUrl;
        return axios.post(url, data, { headers });
    }

    public async PostFormAsync(relativeUrl: string, data: FormData) {
        const url = relativeUrl;
        return axios.post(url, data, {
            headers: {
                "Content-Type": 'multipart/form-data'
            }
        });
    }

    public async PutAsync(relativeUrl: string, data: unknown, headers?: Record<string, string>) {
        const url = relativeUrl;
        return axios.put(url, data, { headers });
    }

    public async DeleteAsync(relativeUrl: string) {
        const url = relativeUrl;
        return axios.delete(url);
    }
}

export default ApiBroker;