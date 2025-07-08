import { FunctionComponent } from "react";
import OdsRowView from "./odsRowView";
import { OdsDataView } from "../../models/views/components/odsData/odsDataView";

type OdsRowProps = {
    ods: OdsDataView;
};

const OdsRow: FunctionComponent<OdsRowProps> = (props) => {
    const {
        ods
    } = props;

    return (
        <OdsRowView
            key={ods.id}
            ods={ods} />
    );
};

export default OdsRow;