import { FunctionComponent } from "react";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";
import ImpersonationContextRowView from "./impersonationContextRowView";

type ImpersonationContextRowProps = {
    impersonationContext: ImpersonationContext;
};

const ImpersonationContextRow: FunctionComponent<ImpersonationContextRowProps> = (props) => {
    const {
        impersonationContext
    } = props;

    return (
        <ImpersonationContextRowView
            key={impersonationContext.id.toString()}
            impersonationContext={impersonationContext}
        />
    );
};

export default ImpersonationContextRow;