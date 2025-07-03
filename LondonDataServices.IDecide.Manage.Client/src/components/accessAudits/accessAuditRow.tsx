import { FunctionComponent } from "react";
import AccessAuditRowView from "./accessAuditRowView";
import { AccessAudit } from "../../models/accessAudit/accessAudit";

type AccessAuditRowProps = {
    accessAudit: AccessAudit;
};

const AccessAuditRow: FunctionComponent<AccessAuditRowProps> = (props) => {
    const {
        accessAudit
    } = props;

    return (
        <AccessAuditRowView
            key={accessAudit.id}
            accessAudit={accessAudit} />
    );
};

export default AccessAuditRow;