import { FunctionComponent } from "react";
import moment from "moment";
import { AccessAudit } from "../../models/accessAudit/accessAudit";

type AccessAuditRowProps = {
    accessAudit: AccessAudit;
};

const AccessAuditRowView: FunctionComponent<AccessAuditRowProps> = (props) => {
    const {
        accessAudit
    } = props
    return (
        <>
            <tr>
                <td><small>{accessAudit.pseudoIdentifier}</small></td>
                <td><small>{accessAudit.givenName} {accessAudit.surname}</small></td>
                <td><small>{accessAudit.email}</small></td>
                <td><small>{accessAudit.reason}</small></td>
                <td><small>{accessAudit.message}</small></td>
                <td><small>{moment(accessAudit.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</small></td>
            </tr>
        </>
    );
}

export default AccessAuditRowView;