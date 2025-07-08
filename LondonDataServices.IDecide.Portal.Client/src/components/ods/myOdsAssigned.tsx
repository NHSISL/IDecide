import { FunctionComponent, useState } from "react";
import { useMsal } from "@azure/msal-react";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";

type myOdsAssignedProps = object;

const MyOdsAssigned: FunctionComponent<myOdsAssignedProps> = () => {
    const account = useMsal();
    const [showSpinner] = useState(false);

    const {
        isLoading,
        data
    } = userAccessViewService.useGetAccessForUser(account.accounts[0].idTokenClaims!.oid!);

    return (
        <>
            {isLoading || showSpinner ? (
                <div>Loading...</div>
            ) : (
                    data && data.map((userAccess: UserAccessView, index: number) => (
                        <>
                            {userAccess.orgCode}
                            {index < data.length - 1 && ","}
                        </>
                    ))
            )}
        </>
    );
};

export default MyOdsAssigned;