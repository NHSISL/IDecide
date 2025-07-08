import { Button, Modal } from "react-bootstrap"
import { BreachDetails } from "./BreachDetails"
import { FunctionComponent } from "react"

type BreachModalProps = {
    show : boolean,
    hide: () => void
}
export const BreachModal : FunctionComponent<BreachModalProps> = ({show, hide}) => {
    return <Modal show={show} size="lg">
        <Modal.Header onHide={hide} closeButton>
            <h4>Re-identification Monitoring</h4>
        </Modal.Header>
        <Modal.Body>
            <BreachDetails/>
        </Modal.Body>
        <Modal.Footer>
            <Button onClick={hide}>OK</Button>
        </Modal.Footer>
    </Modal>

}