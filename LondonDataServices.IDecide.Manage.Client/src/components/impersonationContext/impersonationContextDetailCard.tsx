import React, { FunctionComponent, useState } from "react";
import { Card } from "react-bootstrap";
import ImpersonationContextDetailCardView from "./impersonationContextDetailCardView";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/ImpersonationContextView";

interface ImpersonationContextDetailCardProps {
    impersonationContext: ImpersonationContextView;
    mode: string;
    //onAdd: (impersonationContext: ImpersonationContextView) => void;
    //onUpdate: (impersonationContext: ImpersonationContextView) => void;
    //onDelete: (dataimpersonationContextSet: ImpersonationContextView) => void;
    children?: React.ReactNode;
}

const ImpersonationContextDetailCard: FunctionComponent<ImpersonationContextDetailCardProps> = (props) => {
    const {
        impersonationContext,
        mode,
    } = props;

    const [displayMode, setDisplayMode] = useState<string>(mode);
    //const [apiError, setApiError] = useState<any>({});

    const handleModeChange = (value: string) => {
        setDisplayMode(value);
    };

    // const navigate = useNavigate();

    //const handleAdd = async (dataSet: DataSetView) => {
    //    try {
    //        await onAdd(dataSet);
    //        navigate('/configuration/dataSets');
    //    } catch (error) {
    //        setDisplayMode('EDIT');
    //    }
    //};

    //const handleUpdate = async (dataSet: DataSetView) => {
    //    try {
    //        await onUpdate(dataSet);
    //        setDisplayMode('VIEW');
    //    } catch (error) {
    //        setApiError(error);
    //        setDisplayMode('EDIT');
    //    }
    //};

    //const handleDelete = (dataSet: DataSetView) => {
    //    onDelete(dataSet);
    //    setDisplayMode('VIEW');
    //};

    //const handleCancel = () => {
    //    setApiError({});
    //}

    return (
            <Card>
                <Card.Body>
                    <Card.Title>
                        User Access Detail
                    </Card.Title>

                <Card.Text>
                
                    {(displayMode === "VIEW" || displayMode === "CONFIRMDELETE") && (
                        <>
                            <ImpersonationContextDetailCardView
                                impersonationContext={impersonationContext}
                                onModeChange={handleModeChange}
                                //onDelete={handleDelete}
                                mode={displayMode}
                            />
                        </>
                    )}

                    {/*    {(displayMode === "EDIT" || displayMode === "ADD") && (*/}
                    {/*        <DataSetDetailCardEdit*/}
                    {/*            onModeChange={handleModeChange}*/}
                    {/*            onAdd={handleAdd}*/}
                    {/*            onUpdate={handleUpdate}*/}
                    {/*            onCancel={handleCancel}*/}
                    {/*            dataSet={dataSet}*/}
                    {/*            mode={displayMode}*/}
                    {/*            apiError={apiError}*/}
                    {/*        />*/}
                    {/*    )}*/}
                    {/*    {children !== undefined && (*/}
                    {/*        <>*/}
                    {/*            <br />*/}
                    {/*            {children}*/}
                    {/*        </>*/}
                    {/*    )}*/}
                    </Card.Text>
                </Card.Body>
            </Card>
    );
};

export default ImpersonationContextDetailCard;