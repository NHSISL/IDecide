import { FunctionComponent, ChangeEvent, useState, useEffect } from "react";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { ILookupErrors, LookupErrors } from "./lookupErrors";
import { lookupValidations } from "./lookupValidations";
import { useValidation } from "../../../hooks/useValidation";
import { Button } from "react-bootstrap";
import TextInputBase from "../../bases/inputs/TextInputBase";
import { ILookupApiErrors } from "./lookupApiErrors";

interface LookupRowAddProps {
    onCancel: () => void;
    onAdd: (lookup: LookupView) => void;
    apiError: ILookupApiErrors;
}

const LookupRowAdd: FunctionComponent<LookupRowAddProps> = (props) => {
    const {
        onCancel,
        onAdd,
        apiError
    } = props;

    const [lookup, setLookup] = useState<LookupView>(new LookupView(crypto.randomUUID()));

    const { errors, processApiErrors, enableValidationMessages, validate } =
        useValidation<ILookupErrors,ILookupApiErrors>(LookupErrors, lookupValidations, lookup);

    const handleChange = (event: ChangeEvent<HTMLInputElement> | ChangeEvent<HTMLTextAreaElement>) => {
        const addLookup = {
            ...lookup,
            [event.target.name]: event.target.type === "checkbox"
                ? (event.target as HTMLInputElement).checked : event.target.value,
        };

        setLookup(addLookup);
    };

    const handleSave = () => {
        if (!validate(lookup)) {
            onAdd(lookup);
        } else {
            enableValidationMessages();
        }
    }

    useEffect(() => {
        if(apiError) {
            processApiErrors(apiError);
        }
    }, [apiError, processApiErrors])

    return (
        <tr>
            <td>
                <TextInputBase
                    id="name"
                    name="name"
                    placeholder="Lookup Name"
                    value={lookup.name}
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
                    value={lookup.value}
                    error={errors.value}
                    onChange={handleChange}
                    maxLength={50} />
            </td>
            <td>
                <TextInputBase
                    id="group"
                    name="groupName"
                    placeholder="Lookup Group Value"
                    value={lookup.groupName}
                    required={true}
                    error={errors.groupName}
                    onChange={handleChange}
                    maxLength={50}/>
            </td>
            <td></td>
            <td></td>
            <td className="centre">
                <Button onClick={() => onCancel()} variant="danger">Cancel</Button>&nbsp;
                <Button onClick={() => handleSave()} >Add</Button>
            </td>

        </tr>
    );
}

export default LookupRowAdd;
