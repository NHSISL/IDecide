import { faCircleInfo, faLeftLong, faRightLong, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react";
import { Button, Card, CardBody, Col, Container, Modal, Row, Spinner, Table, Toast, ToastContainer } from "react-bootstrap";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { ReIdRecord } from "../../types/ReIdRecord";
import CopyIcon from "../core/copyIcon";

type ReportToastProps = {
    position: ToastPosition;
    hidden: boolean;
    hide: (hide: boolean) => void;
    clearList: () => void;
    reidentifications: ReIdRecord[];
    reidentificationLoading: boolean;
    lastSelectedPseudo?: ReIdRecord;
    recordLoading: boolean;
    lastPseudos: string[];
    launched: boolean;
    isAuthorised: boolean;
}

const ReportToast: FunctionComponent<ReportToastProps> = (props) => {
    const { position, hidden, hide, reidentifications, lastPseudos, launched, clearList, isAuthorised } = props;
    const [showNoAccessInfo, setShowNoAccessInfo] = useState(false);
    const [pageNumber, setPageNumber] = useState(0);
    const [showHistory, setShowHistory] = useState(false);
    const [bulkModalShown, setBulkModalShown] = useState(true);
    const itemsPerPage = 10;
    const clipboardAvailable = navigator.clipboard;

    function getSingleRecordCard(reidRecords: ReIdRecord[], pseudoToShow: string) {
        const reidRecord = reidRecords.find(rec => pseudoToShow === rec.pseudo);

        if (!reidRecord) {
            return <Card>
                <CardBody><Spinner /></CardBody>
            </Card>
        }

        if (!reidRecord.hasAccess) {
            return <Card bg="danger" text="white" className="mb-1" key={reidRecord.pseudo}>
                <CardBody>
                    You do not have permissions to re-identify {reidRecord.pseudo}.
                </CardBody>
            </Card>
        }

        return <Card bg="success" text="white" className="mb-1" key={reidRecord.pseudo}>
            <Card.Body>
                <b>Pseudo:&nbsp;</b>
                {reidRecord.isHx ? reidRecord.pseudo : '--'}&nbsp;
                <b>NHS:</b> {reidRecord.nhsnumber}&nbsp;&nbsp;
                {clipboardAvailable &&
                    <CopyIcon content={reidRecord.nhsnumber || ""} resetTime={2000} />
                }
            </Card.Body>
        </Card>
    }

    function getNhsNumbers(listOfPseudos: string[], reIdRecords: ReIdRecord[]) {
        const records = reIdRecords
            .filter(x => listOfPseudos.indexOf(x.pseudo) !== -1)
            .flatMap(x => `${x.isHx ? x.pseudo : 'PseudoNumberRedacted'},  ${x.hasAccess ? x.nhsnumber : 'NHSNumberRedacted'}`)
        return [... new Set(records)].join('\n'); // shortcut to return distinct list of values
    }

    function getNhsNumbersHistory() {
        const records = reidentifications
            .flatMap(x => `${x.isHx ? x.pseudo : 'PseudoNumberRedacted'},  ${x.hasAccess ? x.nhsnumber : 'NHSNumberRedacted'}`)
        return [... new Set(records)].join('\n'); 
    }

    function getMultiRecordCard(listOfPseudos: string[], reIdRecords: ReIdRecord[]) {
        if (listOfPseudos.length > 5) {
            return <><Modal show={bulkModalShown} scrollable onHide={() => { setBulkModalShown(false) }} >
                <Modal.Header closeButton>
                    Re-id Records
                </Modal.Header>
                <Modal.Body>
                    {listOfPseudos.map(ls => <div key={ls}> {getSingleRecordCard(reIdRecords, ls)} </div>)}
                </Modal.Body>
                <Modal.Footer>
                    <CopyIcon iconText="Copy All" content={getNhsNumbers(listOfPseudos, reIdRecords)} resetTime={2000} />
                </Modal.Footer>
            </Modal>
                {!bulkModalShown && <div>
                    <p>You have requested to re-identify {listOfPseudos.length} identifiers which is too many to display here.</p>
                    <Button onClick={() => { setBulkModalShown(true) }}>Show Bulk Re-id Screen</Button>
                </div>}
            </>
        }

        return <>
            {listOfPseudos.map((ls) => <div key={ls}>{getSingleRecordCard(reIdRecords, ls)}</div>)}
        </>
    }


    return <ToastContainer position={position} hidden={hidden || !launched || !isAuthorised} className="me-2" >
        <Toast onClose={() => hide(true)} bg="light">
            <Toast.Header>
                <strong className="me-auto">Re-identifications</strong>
            </Toast.Header>
            <Toast.Body>
                <Container>
                    <Row>
                        {lastPseudos.length > 1 && <>
                            <Col xs={8}>
                                <h5>Selected Records</h5>
                            </Col>
                            <Col className="text-end">
                                <CopyIcon iconText="Copy" content={getNhsNumbers(lastPseudos, reidentifications)} resetTime={2000} />
                            </Col>
                        </>
                        }

                    </Row>
                    <Row>
                        {lastPseudos.length === 1 && <>
                            {getSingleRecordCard(reidentifications, lastPseudos[0])}
                        </>}

                        {lastPseudos.length > 1 && <>
                            {getMultiRecordCard(lastPseudos, reidentifications)}
                        </>}

                    </Row>


                    {showHistory &&
                        <> <hr className="mt-2 border-dark border-5" />
                            <Row>
                                <Col xs={8}>
                                    <h5>History</h5>
                                </Col>
                                <Col className="text-end">
                                    <CopyIcon iconText="Copy" content={getNhsNumbersHistory()} resetTime={2000} />
                                </Col>
                            </Row>
                            <Table size="sm" bordered>
                                <tbody>
                                    {reidentifications.slice(pageNumber * itemsPerPage, (pageNumber * itemsPerPage) + itemsPerPage).map((ri) => <tr key={crypto.randomUUID()}>
                                        <td>{ri.isHx ? ri.pseudo : "---"}</td>
                                        {ri.loading ? <td>
                                            <FontAwesomeIcon icon={faSpinner} pulse />
                                        </td> : <>
                                            {ri.hasAccess ? <>
                                                <td>{ri.nhsnumber}</td><td><CopyIcon content={ri.nhsnumber || ""} resetTime={1000} />
                                                </td>
                                            </>
                                                : <td colSpan={2}>
                                                    NO ACCESS <FontAwesomeIcon icon={faCircleInfo} color="red" onClick={() => setShowNoAccessInfo(true)} />
                                                </td>}
                                        </>
                                        }

                                    </tr>)}
                                </tbody>
                        </Table>
                        {reidentifications.length > itemsPerPage && <Row className="mb-2">
                                <Col className="d-grid gap-2">
                                    <Button onClick={() => {
                                        if (pageNumber !== 0)
                                            setPageNumber(pageNumber - 1);
                                    }}>
                                        <FontAwesomeIcon icon={faLeftLong} />
                                    </Button>
                                </Col>
                                <Col>
                                    <b>Page: {pageNumber + 1} of {Math.ceil(reidentifications.length / itemsPerPage)} </b>
                                </Col>
                                <Col className="d-grid gap-2">
                                    <Button onClick={() => {
                                        if (pageNumber + 1 !== Math.ceil(reidentifications.length / itemsPerPage))
                                            setPageNumber(pageNumber + 1)
                                    }}>
                                        <FontAwesomeIcon icon={faRightLong} />
                                    </Button>
                                </Col>
                            </Row>
                            }
                        </>
                    }

                    {showHistory && reidentifications.length > 0 &&
                        <Row>
                            <Col>
                            </Col>
                            <Col className="d-grid">

                                <Button size="sm" variant="outline-primary" onClick={() => {
                                    setShowHistory(false);
                                    clearList();
                                }}
                                >Clear History</Button>
                            </Col>
                        </Row>
                    }
                    <Row className="mt-2">
                        <Col className="d-grid">
                            <Button size="sm" onClick={() => setShowHistory(!showHistory)}>{showHistory ? 'Hide' : 'Show'} reidentification history</Button>
                        </Col>
                    </Row>
                </Container>
            </Toast.Body>
        </Toast>
        <Modal show={showNoAccessInfo} onHide={() => { setShowNoAccessInfo(false) }}>
            <Modal.Header closeButton>You do not have access to this Patient Record</Modal.Header>
            <Modal.Body>
                <p>You have tried to re-identify a patient's that our records indicate that you do not have access to.</p>
                <p>Check that the patient is registered to an GP practice that you have access to.</p>
                <p>To view your ODS organisations configured in the re-identification tool click <a href="about:blank">here</a> and contact your local ICB should you need further access.</p>
                <p>Any changes to the patient record regisistration will take 24 hours to apply to the re-identification service </p>
            </Modal.Body>
        </Modal>
    </ToastContainer>
}

export default ReportToast