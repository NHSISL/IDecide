import { FunctionComponent } from "react";
import moment from "moment";
import { AccessAudit } from "../../models/accessAudit/accessAudit";
import { Badge } from "react-bootstrap";

type AccessAuditRowProps = {
    accessAudit: AccessAudit;
    okCount: number;
    notOkCount: number;
};

const AccessAuditRowView: FunctionComponent<AccessAuditRowProps> = (props) => {
    const {
        accessAudit,
        okCount,
        notOkCount
    } = props
    return (
        <>
            <tr>
                <td className="align-middle"><small>{accessAudit.givenName} {accessAudit.surname}</small></td>
                <td className="align-middle"><small>{accessAudit.email}</small></td>
                <td className="align-middle"><small>{moment(accessAudit.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</small></td>
                <td>
                    <div>
                        <Badge bg="primary" className="me-1">
                            Total Rows: {okCount + notOkCount}
                        </Badge>
                    </div>
                    <div>
                        <Badge bg="success" className="me-1">
                            Total Re-Identified: {okCount}
                        </Badge>
                    </div>
                    <div>
                        <Badge bg="danger" className="me-1">
                            Total Not Re-Identified: {notOkCount}
                        </Badge>
                    </div>
                </td>

            </tr>
        </>
    );
}

export default AccessAuditRowView;