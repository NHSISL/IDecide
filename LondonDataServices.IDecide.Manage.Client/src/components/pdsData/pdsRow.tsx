import { FunctionComponent } from "react";
import PdsRowView from "./pdsRowView";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";

type PdsRowProps = {
    pds: PdsDataView;
};

const PdsRow: FunctionComponent<PdsRowProps> = (props) => {
    const {
        pds
    } = props;

    return (
        <PdsRowView
            key={pds.id}
            pds={pds} />
    );
};

export default PdsRow;