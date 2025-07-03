import { FunctionComponent } from "react";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { Button } from "react-bootstrap";
import { toastSuccess } from "../../../brokers/toastBroker.success";
import moment from "moment";

interface LookupRowDeleteProps {
    lookup: LookupView;
    onCancel: (id: string) => void;
    onDelete: (lookup: LookupView) => void;
}

const LookupRowDelete: FunctionComponent<LookupRowDeleteProps> = (props) => {
    const {
        lookup,
        onCancel,
        onDelete
    } = props;

    const handleDelete = (lookup: LookupView) => {
        onDelete(lookup);
        toastSuccess(`${lookup.name} Deleted`);
    }

    return (
        <tr>
            <td>
                <b>{lookup.name}</b>
            </td>
            <td>
                {lookup.value}
            </td>
            <td>{lookup.groupName}</td>
            <td>{lookup.createdBy}</td>
            <td>{moment(lookup.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            <td>

                <Button onClick={() => onCancel(lookup.id)} variant="warning">Cancel</Button>&nbsp;
                <Button onClick={() => handleDelete(lookup)} variant="danger">Yes, Delete</Button>
            </td>
        </tr>
    );
}

export default LookupRowDelete;

