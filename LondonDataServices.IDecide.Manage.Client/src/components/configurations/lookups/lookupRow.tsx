import { FunctionComponent, useState } from "react";
import LookupRowView from "./lookupRowView";
import LookupRowEdit from "./lookupRowEdit";
import LookupRowDelete from "./lookupRowDelete";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { ILookupErrors, LookupErrors } from "./lookupErrors";

type LookupRowProps = {
    lookup: LookupView;
    allowedToEdit: boolean;
    allowedToDelete: boolean;
    onUpdate: (lookup: LookupView) => void;
    onDelete: (lookup: LookupView) => void;
}

const LookupRow: FunctionComponent<LookupRowProps> = (props) => {
    const {
        lookup,
        allowedToEdit,
        allowedToDelete,
        onUpdate,
        onDelete,
    } = props;

    const [mode, setMode] = useState<string>('VIEW');
    const [apiError, setApiError] = useState<ILookupErrors>(LookupErrors);


    const handleMode = (value: string) => {
        setMode(value);
    };

    const handleUpdate = async (lookup: LookupView) => {
        try {
            await onUpdate(lookup);
            setMode('VIEW');
        } catch (error) {
            setApiError(error as ILookupErrors);
            setMode('EDIT');
        }
    };

    const handleDelete = (lookup: LookupView) => {
        console.log(lookup);
        onDelete(lookup);
        setMode('VIEW');
    };

    const handleCancel = () => {
        setMode('VIEW');
    };

    return (
        <>
            {mode !== 'EDIT' && mode !== 'DELETE' && (
                <LookupRowView
                    key={lookup.id.toString()}
                    lookup={lookup}
                    onEdit={handleMode}
                    onDelete={handleMode}
                    allowedToEdit={allowedToEdit}
                    allowedToDelete={allowedToDelete} 
                />
            )}
            {mode === 'EDIT' && (
                <LookupRowEdit
                    key={lookup.id.toString()}
                    lookup={lookup}
                    onCancel={handleCancel}
                    onEdit={handleUpdate}
                    apiError={apiError}
                />
            )}

            {mode === 'DELETE' && (
                <LookupRowDelete
                    key={lookup.id.toString()}
                    lookup={lookup}
                    onCancel={handleCancel}
                    onDelete={handleDelete} />
            )}

        </>
    );
}

export default LookupRow;