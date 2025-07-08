import { FunctionComponent } from "react";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";

type CsvIdentificationRequestRowProps = {
    csvIdentificationRequest: CsvIdentificationRequest;
}

const CsvIdentificationRequestRow: FunctionComponent<CsvIdentificationRequestRowProps> = (props) => {
    const {
        csvIdentificationRequest
    } = props;


    return (
        <tr>
            <td>{csvIdentificationRequest.requesterFirstName} {csvIdentificationRequest.requesterLastName}</td>
            <td>{csvIdentificationRequest.requesterEmail}</td>
            <td>{csvIdentificationRequest.recipientFirstName} {csvIdentificationRequest.recipientLastName}</td>
            <td>{csvIdentificationRequest.reason}</td>
            <td>{csvIdentificationRequest.filepath}</td>
        </tr>
    );
}

export default CsvIdentificationRequestRow;