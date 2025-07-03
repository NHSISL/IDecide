import { FunctionComponent } from "react";
import { CsvIdentificationRequestView } from "../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";

interface CsvIdentificationRequestDetailCardViewProps {
    csvIdentificationRequest: CsvIdentificationRequestView;
    //onDelete: (csvIdentificationRequest: CsvIdentificationRequestView) => void;
    mode: string;
    onModeChange: (value: string) => void;
}

const CsvIdentificationRequestDetailCardView: FunctionComponent<CsvIdentificationRequestDetailCardViewProps> = (props) => {
    const {
        csvIdentificationRequest,
    } = props;


    return (
        <>
            <h1>To Build the CSV Identidication Detail View Page</h1>
            {csvIdentificationRequest.id}
            
        </>
    );
}

export default CsvIdentificationRequestDetailCardView;
