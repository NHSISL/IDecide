import { FunctionComponent } from "react";
import ConsumerRowView from "./consumerRowView";
import ConsumerRowAdd from "./consumerRowAdd";
import { Consumer } from "../../models/consumers/consumer";

type ConsumerRowProps = {
    consumer?: Consumer;
    action?: "view" | "add";
    onCancel?: () => void;
    onSave?: (consumer: Consumer) => void;
    onEdit?: () => void;
    onDelete?: (id: string) => void;
};

const ConsumerRow: FunctionComponent<ConsumerRowProps> = (props) => {
    const { consumer, action = "view", onCancel,onSave, onEdit, onDelete } = props;

    if (action === "add") {
        return <ConsumerRowAdd onCancel={onCancel} onSave={onSave} />;
    }

    return (
        <>
            {consumer && (
                <ConsumerRowView
                    key={consumer.id}
                    consumer={consumer}
                    onEdit={onEdit}
                    onDelete={onDelete}
                />
            )}
        </>
    );
};

export default ConsumerRow;