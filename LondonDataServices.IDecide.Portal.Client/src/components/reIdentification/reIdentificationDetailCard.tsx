import { FunctionComponent } from "react";
import { Card } from "react-bootstrap";
import ReIdentificationDetailCardView from "./reIdentificationDetailCardView";
import { LookupView } from "../../models/views/components/lookups/lookupView";

interface ReIdentificationDetailCardProps {
    lookups: Array<LookupView>;
}

const ReIdentificationDetailCard: FunctionComponent<ReIdentificationDetailCardProps> = (props) => {
    const {
        lookups,
    } = props;

    return (
        <Card style={{ width: '50rem' }} className="m-0 p-0">
            <ReIdentificationDetailCardView
                lookups={lookups}
            />
        </Card>
    );
};

export default ReIdentificationDetailCard;