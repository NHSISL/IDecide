import { FunctionComponent } from "react";
import AccessAuditRowView from "./accessAuditRowView";
import { AccessAudit } from "../../models/accessAudit/accessAudit";

type AccessAuditRowProps = {
    accessAudit: AccessAudit;
    okCount: number;
    notOkCount: number;
};

const AccessAuditRow: FunctionComponent<AccessAuditRowProps> = (props) => {
    const {
        accessAudit,
        okCount,
        notOkCount
    } = props;

    return (
        <AccessAuditRowView
            key={accessAudit.id}
            accessAudit={accessAudit}
            okCount={okCount}
            notOkCount={notOkCount} />
    );
};

export default AccessAuditRow;