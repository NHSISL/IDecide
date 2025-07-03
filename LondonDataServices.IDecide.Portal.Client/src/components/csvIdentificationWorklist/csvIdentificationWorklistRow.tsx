import { FunctionComponent } from "react";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";
import CsvIdentificationRequestRowView from "./csvIdentificationWorklistRowView";

type CsvIdentificationWorklistRowProps = {
    csvIdentificationRequest: CsvIdentificationRequest;
};

const CsvIdentificationWorklistRow: FunctionComponent<CsvIdentificationWorklistRowProps> = (props) => {
    const {
        csvIdentificationRequest
    } = props;

    return (
        <CsvIdentificationRequestRowView
            key={csvIdentificationRequest.id!.toString()}
            csvIdentificationRequest={csvIdentificationRequest}
        />
    );
};

export default CsvIdentificationWorklistRow;