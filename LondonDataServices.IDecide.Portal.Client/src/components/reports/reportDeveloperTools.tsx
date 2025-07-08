import { faLeftLong, faRightLong } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent } from "react";
import { Accordion, Button, ButtonGroup } from "react-bootstrap"
import { DeveloperEvents } from "../../types/DeveloperEvents";

type ReportDeveloperToolsProps = {
    developerToolsLocation: string,
    setDeveloperToolsLocation: (location: string) => void;
    eventsList: DeveloperEvents[];
}

const ReportDeveloperTools: FunctionComponent<ReportDeveloperToolsProps> = (props: ReportDeveloperToolsProps) => {
    const { developerToolsLocation, setDeveloperToolsLocation, eventsList } = props;

    return <>{developerToolsLocation &&
        <div className={`offcanvas offcanvas-${developerToolsLocation} show`} >
            <ButtonGroup size="sm">
                <Button onClick={() => { setDeveloperToolsLocation("start") }}><FontAwesomeIcon icon={faLeftLong} /></Button>
                <Button onClick={() => { setDeveloperToolsLocation("end") }}><FontAwesomeIcon icon={faRightLong} /></Button>
                <Button onClick={() => { setDeveloperToolsLocation("") }}>Close</Button>
            </ButtonGroup>
            <div style={{overflowY: "auto"}}>
                <Accordion>
                    {eventsList.map((e, i) => <Accordion.Item eventKey={"" + i}>
                        <Accordion.Header>{e.message}</Accordion.Header>
                        <Accordion.Body>
                            <>{e.eventDetails && <pre>{JSON.stringify(e.eventDetails.detail, null, 2)}</pre>}</>
                        </Accordion.Body>
                    </Accordion.Item>
                    )}
                </Accordion>
            </div>
        </div>
    }</>
}

export default ReportDeveloperTools