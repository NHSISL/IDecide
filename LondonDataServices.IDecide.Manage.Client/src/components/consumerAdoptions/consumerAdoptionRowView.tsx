import { FunctionComponent } from "react";
import { ConsumerAdoption } from "../../models/consumerAdoptions/consumerAdoption";
import moment from "moment";

type ConsumerAdoptionRowProps = {
    consumerAdoption: ConsumerAdoption;
};

const ConsumerAdoptionRowView: FunctionComponent<ConsumerAdoptionRowProps> = (props) => {
    const {
        consumerAdoption
    } = props

    return (
        <>
            <tr>
                <td>{consumerAdoption.consumer?.name}</td>
                <td>{consumerAdoption.decision?.decisionChoice}</td>
                <td>{consumerAdoption.decision?.createdDate ? moment(consumerAdoption.decision?.createdDate).format("DD-MM-YYYY HH:mm") : ""}</td>
                <td>{consumerAdoption.adoptionDate ? moment(consumerAdoption.adoptionDate).format("DD-MM-YYYY HH:mm") : ""}</td>
            </tr>
        </>
    );
}

export default ConsumerAdoptionRowView;