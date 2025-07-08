import { FunctionComponent } from "react";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";
import CsvIdentificationRequestRowView from "./csvIdentificationRequestRowView";

type CsvIdentificationRequestRowProps = {
    csvIdentificationRequest: CsvIdentificationRequest;
};

const CsvIdentificationRequestRow: FunctionComponent<CsvIdentificationRequestRowProps> = (props) => {
    const {
        csvIdentificationRequest
    } = props;

    return (
        <CsvIdentificationRequestRowView
            key={csvIdentificationRequest.id.toString()}
            csvIdentificationRequest={csvIdentificationRequest}
        />
    );
};

export default CsvIdentificationRequestRow;