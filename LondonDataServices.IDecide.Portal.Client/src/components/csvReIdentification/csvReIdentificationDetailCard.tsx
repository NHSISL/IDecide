import { FunctionComponent } from "react";
import { Card } from "react-bootstrap";
import CsvReIdentificationDetailCardView from "./csvReIdentificationDetailCardView";

const CsvReIdentificationDetailCard: FunctionComponent = () => {

    return (
        <Card style={{ width: '50rem' }} className="m-0 p-0">
            <CsvReIdentificationDetailCardView />
        </Card>
    );
};

export default CsvReIdentificationDetailCard;