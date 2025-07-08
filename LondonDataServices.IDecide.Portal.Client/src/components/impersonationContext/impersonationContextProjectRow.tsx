import { FunctionComponent } from "react";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";
import ImpersonationContextProjectRowView from "./impersonationContextProjectRowView";

type ImpersonationContextProjectRowProps = {
    impersonationContext: ImpersonationContext;
};

const ImpersonationContextProjectRow: FunctionComponent<ImpersonationContextProjectRowProps> = (props) => {
    const {
        impersonationContext
    } = props;

    return (
        <ImpersonationContextProjectRowView
            key={impersonationContext.id.toString()}
            impersonationContext={impersonationContext}
        />
    );
};

export default ImpersonationContextProjectRow;