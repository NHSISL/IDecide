import { FunctionComponent, useState } from "react";
import { Consumer } from "../../models/consumers/consumer";
import { Button } from "react-bootstrap";

type ConsumerRowProps = {
    consumer: Consumer;
    onEdit?: () => void;
    onDelete?: (id: string) => void;
};

const ConsumerRowView: FunctionComponent<ConsumerRowProps> = (props) => {
    const {
        consumer
        , onEdit
        , onDelete
    } = props

    const [confirmDelete, setConfirmDelete] = useState(false);

    return (
        <>
            <tr>
                <td>{consumer.name}</td>
                <td>{consumer.contactEmail}</td>
                <td>{consumer.contactNumber}</td>
                <td>{consumer.contactPerson}</td>
                <td>{consumer.entraId}</td>
                <td className="text-center">
                    {!confirmDelete ? (
                        <>
                            <Button variant="primary" size="sm" onClick={onEdit}>
                                Edit
                            </Button>
                            <Button
                                variant="danger"
                                size="sm"
                                className="ms-2"
                                onClick={() => setConfirmDelete(true)}
                            >
                                Delete
                            </Button>
                        </>
                    ) : (
                        <>
                            <Button
                                variant="outline-danger"
                                size="sm"
                                onClick={() => onDelete && onDelete(consumer.id ?? "")}
                            >
                                Confirm
                            </Button>
                            <Button
                                variant="secondary"
                                size="sm"
                                className="ms-2"
                                onClick={() => setConfirmDelete(false)}
                            >
                                Cancel
                            </Button>
                        </>
                    )}
                </td>
            </tr>
        </>
    );
}

export default ConsumerRowView;