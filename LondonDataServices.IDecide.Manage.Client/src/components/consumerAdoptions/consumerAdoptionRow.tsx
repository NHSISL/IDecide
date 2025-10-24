import { FunctionComponent } from "react";
import ConsumerAdoptionRowView from "./consumerAdoptionRowView";
import { ConsumerAdoption } from "../../models/consumerAdoptions/consumerAdoption";

type ConsumerAdoptionRowProps = {
    consumerAdoption?: ConsumerAdoption;
};

const ConsumerAdoptionRow: FunctionComponent<ConsumerAdoptionRowProps> = (props) => {
    const { consumerAdoption } = props;

    return (
        <>
            {consumerAdoption && (
                <ConsumerAdoptionRowView
                    key={consumerAdoption.id}
                    consumerAdoption={consumerAdoption}
                />
            )}
        </>
    );
};

export default ConsumerAdoptionRow;