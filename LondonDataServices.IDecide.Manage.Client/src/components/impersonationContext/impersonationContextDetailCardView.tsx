import { FunctionComponent } from "react";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/ImpersonationContextView";

interface ImpersonationContextDetailCardViewProps {
    impersonationContext: ImpersonationContextView;
    //onDelete: (impersonationContext: ImpersonationContextView) => void;
    mode: string;
    onModeChange: (value: string) => void;
}

const ImpersonationContextDetailCardView: FunctionComponent<ImpersonationContextDetailCardViewProps> = (props) => {
    const {
        impersonationContext,
    } = props;


    return (
        <>
            <h1>To Build the Impersonation Detail View Page</h1>
            {impersonationContext.id}
            
        </>
    );
}

export default ImpersonationContextDetailCardView;
