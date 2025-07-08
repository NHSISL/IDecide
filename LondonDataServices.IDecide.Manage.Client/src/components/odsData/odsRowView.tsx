import { FunctionComponent } from "react";
import { OdsDataView } from "../../models/views/components/odsData/odsDataView";
import moment from "moment";
import { faCheck, faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

type OdsRowProps = {
    ods: OdsDataView;
};

const OdsRow: FunctionComponent<OdsRowProps> = (props) => {
    const {
        ods
    } = props
    return (
        <>
            <tr>
                <td>{ods.organisationName}</td>
                <td>{ods.organisationCode}</td>
                <td>
                    {ods.hasChildren ?
                        <FontAwesomeIcon icon={faCheck} className="text-success" /> :
                        <FontAwesomeIcon icon={faTimes} className="text-danger" />}
                </td>
                <td>{moment(ods.relationshipWithParentStartDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
                <td>{moment(ods.relationshipWithParentEndDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            </tr>
        </>
    );
}

export default OdsRow;