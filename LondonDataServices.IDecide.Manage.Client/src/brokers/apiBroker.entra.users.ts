import { User } from "../models/views/components/Users/Users";
import ApiBroker from "./apiBroker";

class UsersBroker {
    absoluteUrl = 'https://graph.microsoft.com/v1.0/users';
    scope = 'Directory.Read.All'
    
    private apiBroker: ApiBroker = new ApiBroker(this.scope);

    async FilterUsersAsync(emailAddressFragment: string) {
        if(!emailAddressFragment) {
            return [];
        }

        const url = `${this.absoluteUrl}?$filter=startswith(mail,'${emailAddressFragment}')`;

        return await this.apiBroker.GetAsyncAbsolute(url)
            .then(result => {
                if(result.data && result.data.value) {
                    return result.data.value as User[];
                }
            });
    }
}
export default UsersBroker;