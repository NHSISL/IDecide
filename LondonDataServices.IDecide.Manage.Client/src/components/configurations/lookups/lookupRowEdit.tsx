import { FunctionComponent, ChangeEvent, useState, useEffect } from "react";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { ILookupErrors, LookupErrors } from "./lookupErrors";
import { lookupValidations } from "./lookupValidations";
import TextInputBase from "../../bases/inputs/TextInputBase";
import { Button } from "react-bootstrap";
import { useValidation } from "../../../hooks/useValidation";

interface LookupRowEditProps {
    lookup: LookupView;
    onCancel: () => void;
    onEdit: (supplier: LookupView) => void;
    apiError: ILookupErrors;
}

const LookupRowEdit: FunctionComponent<LookupRowEditProps> = (props) => {
    const {
        lookup,
        onCancel,
        onEdit,
        apiError
    } = props;

    const [editLookup, setEditLookup] = useState<LookupView>({ ...lookup });

    const { errors, processApiErrors, enableValidationMessages, validate } =
        useValidation(LookupErrors, lookupValidations, editLookup);

    const handleChange = (event: ChangeEvent<HTMLInputElement> | ChangeEvent<HTMLTextAreaElement>) => {
        const updatedLookup = {
            ...editLookup,
            [event.target.name]: event.target.type === "checkbox"
                ? (event.target as HTMLInputElement).checked : event.target.value,
        };

        setEditLookup(updatedLookup);
    };

    const handleCancel = () => {
        setEditLookup({ ...lookup });
        onCancel();
    };

    const handleUpdate = () => {
        if (!validate(editLookup)) {
            onEdit(editLookup);
        } else {
            enableValidationMessages();
        }
    }

    useEffect(() => {
        processApiErrors(apiError);
    }, [apiError, processApiErrors])

    return (
        <tr>
            <td>
                <TextInputBase
                    id="name"
                    name="name"
                    placeholder="Lookup Name"
                    value={editLookup.name}
                    required={true}
                    error={errors.name}
                    onChange={handleChange}
                    maxLength={50} />
            </td>
            <td>
                <TextInputBase
                    id="value"
                    name="value"
                    placeholder="Lookup Value"
                    value={editLookup.value}
                    required={true}
                    error={errors.name}
                    onChange={handleChange}
                    maxLength={50} />
            </td>
            <td>
                <TextInputBase
                    id="group"
                    name="groupName"
                    placeholder="Lookup Group Value"
                    value={editLookup.groupName}
                    required={true}
                    error={errors.groupName}
                    onChange={handleChange}
                    maxLength={50} />
            </td>
            <td></td>
            <td></td>
            <td>
                <Button onClick={() => handleCancel()} variant="danger">Cancel</Button>&nbsp;
                <Button onClick={() => handleUpdate()} disabled={errors.hasErrors} >Update</Button>
            </td>
        </tr>
    );
}

export default LookupRowEdit;