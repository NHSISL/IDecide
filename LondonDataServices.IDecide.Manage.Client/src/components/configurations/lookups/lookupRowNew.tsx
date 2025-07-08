import { FunctionComponent } from "react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCirclePlus } from '@fortawesome/free-solid-svg-icons'
import { Button } from "react-bootstrap";
import { SecuredComponent } from "../../securitys/securedComponents";
import securityPoints from "../../../securityMatrix";

interface LookupRowNewProps {
    onAdd: (value: boolean) => void;
}

const LookupRowNew: FunctionComponent<LookupRowNewProps> = (props) => {
    const {
        onAdd
    } = props;

    return (
        <SecuredComponent>
            <tr>
                <td colSpan={5}></td>

                <td>
                    <SecuredComponent allowedRoles={securityPoints.configuration.add}>
                        <Button id="lookupAdd" onClick={() => onAdd(true)} variant="success">
                            <FontAwesomeIcon icon={faCirclePlus} size="lg" />&nbsp; New
                        </Button>
                    </SecuredComponent>
                </td>
            </tr>
        </SecuredComponent>
    );
}

export default LookupRowNew;
