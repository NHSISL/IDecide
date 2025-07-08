import { FunctionComponent } from "react";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import UserAccessRowView from "./userAccessRowView";

type UserAccessRowProps = {
    userAccess: UserAccessView;
};

const UserAccessRow: FunctionComponent<UserAccessRowProps> = (props) => {
    const {
        userAccess
    } = props;

    return (
        <UserAccessRowView
            key={userAccess.id}
            userAccess={userAccess} />
    );
};

export default UserAccessRow;