import { FunctionComponent } from "react";
import securityPoints from "../../securityMatrix";
import { SecuredComponent } from "../securitys/securedComponents";
import { Link } from "react-router-dom";
import { Button } from "react-bootstrap";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";

type UserAccessRowProps = {
    userAccess: UserAccessView;
};

const UserAccessRow: FunctionComponent<UserAccessRowProps> = (props) => {
    const {
        userAccess
    } = props
    return (
        <>
            <tr>
                <td>{userAccess.displayName}</td>
                <td>{userAccess.email}</td>
                <td>
                    <SecuredComponent allowedRoles={securityPoints.userAccess.edit}>
                        <Link to={`/userAccess/${userAccess.UserId}`} >
                            <Button size="sm">
                                Edit
                            </Button>
                        </Link>
                    </SecuredComponent>
                </td>
            </tr>
        </>
    );
}

export default UserAccessRow;