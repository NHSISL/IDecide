import { FunctionComponent } from "react";
import { Link } from 'react-router-dom';
import { Button } from "react-bootstrap";
import { faCheck, faClock } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";
import { useMsal } from "@azure/msal-react";

type ImpersonationContextProjectRowProps = {
    impersonationContext: ImpersonationContext;
}

const ImpersonationContextProjectRow: FunctionComponent<ImpersonationContextProjectRowProps> = (props) => {
    const {
        impersonationContext
    } = props;

    const { accounts } = useMsal();
    const account = accounts[0];

    if (
        account.idTokenClaims?.oid?.toLowerCase() !== impersonationContext?.responsiblePersonUserId.toLowerCase() &&
        account.idTokenClaims?.oid?.toLowerCase() !== impersonationContext?.requesterUserId.toLowerCase()
    ) {
        return null;
    }

    return (
        <tr>
            <td>{impersonationContext.projectName}</td>
            <td>
                {impersonationContext.isApproved
                    ? <>
                        <FontAwesomeIcon icon={faCheck} className="text-success" /> Approved
                    </>
                    : <>
                        <FontAwesomeIcon icon={faClock} className="text-warning" /> Pending Approval
                    </>
                }
            </td>
            <td>
                <Link to={`/project/${impersonationContext.id}`}>
                    <Button onClick={() => { }}>
                        Manage
                    </Button>
                </Link>
            </td>
        </tr>
    );
}

export default ImpersonationContextProjectRow;