import React, { FunctionComponent, useState, useEffect } from "react";
import ImpersonationContextDetailCard from "./impersonationContextDetailCard";
import { useParams } from "react-router-dom";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/ImpersonationContextView";
import { impersonationContextViewService } from "../../services/views/impersonationContext/impersonationContextViewService";

type ImpersonationContextDetailProps = {
    children?: React.ReactNode;
};

const ImpersonationContextDetail: FunctionComponent<ImpersonationContextDetailProps> = (props) => {
    const {
        children
    } = props;

    const { ImpersonationContextId } = useParams();

    let impersonationContextRetrieved: ImpersonationContextView | undefined

    if (ImpersonationContextId) {
        const { mappedImpersonationContext } = impersonationContextViewService.useGetImpersonationContextById(ImpersonationContextId);
        impersonationContextRetrieved = mappedImpersonationContext
    }

    const [impersonationContext, setImpersonationContext] = useState<ImpersonationContextView>();
    const [mode, setMode] = useState<string>('VIEW');

    useEffect(() => {
        if (ImpersonationContextId !== "") {
            setImpersonationContext(impersonationContextRetrieved);
            setMode('VIEW');
        }
        if (ImpersonationContextId === "" || ImpersonationContextId === undefined) {
            setImpersonationContext(new ImpersonationContextView(crypto.randomUUID(), "", "", "", "", "","","","","",false,"","",new Date(),"",new Date()))
            setMode('ADD');
        }
    }, [ImpersonationContextId, impersonationContextRetrieved]);

    return (
        <div>
            {impersonationContext !== undefined && (
                <div>
                    <ImpersonationContextDetailCard
                        key={ImpersonationContextId}
                        impersonationContext={impersonationContext}
                        mode={mode}
                        //onAdd={handleAdd}
                        //onUpdate={handleUpdate}
                        //onDelete={handleDelete}
                    >
                        {children}
                    </ImpersonationContextDetailCard>
                </div>
            )}
        </div>
    );
};

export default ImpersonationContextDetail;