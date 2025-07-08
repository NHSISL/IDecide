import { FunctionComponent } from "react";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";
import { convertToHx } from "../../helpers/hxHelpers";
import CopyIcon from "../core/copyIcon";

type PdsRowProps = {
    pds: PdsDataView;
};

const PdsRow: FunctionComponent<PdsRowProps> = (props) => {
    const { pds } = props;


    return (
        <>
            <tr>
                <td>
                    {pds.pseudoNhsNumber}
                    <CopyIcon content={pds.pseudoNhsNumber || ""} resetTime={2000} />
                </td>
                <td>
                    {convertToHx(pds.pseudoNhsNumber)}
                    <CopyIcon content={convertToHx(pds.pseudoNhsNumber) || ""} resetTime={2000} />
                </td>
                <td>{pds.orgCode}</td>
            </tr>
        </>
    );
}

export default PdsRow;