
import { PowerBIEmbed } from "powerbi-client-react";
import { FunctionComponent, useCallback, useEffect, useState } from "react";
import ReportToast from "./reportToast";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { useParams } from "react-router-dom";
import { PBIEvent, PBIIdentity, PBIValues } from "../../types/PBIEvent";
import { useReidentification } from "../../hooks/useReidentification";
import { Button, Col, Modal, Row } from "react-bootstrap";
import FakeReportPage from "./fakeReportPage";
import { ToastContainer } from "react-toastify";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";
import { BreachDetails } from "../breachDetails/BreachDetails";
import { IReportEmbedConfiguration } from "embed";
import { ReportObject } from "../../types/ReportObject";
import { toastInfo } from "../../brokers/toastBroker.info";


type ReportLaunchPageProps = {
    reportConfig: IReportEmbedConfiguration
    addDeveloperEvent: (event: DeveloperEvents) => void;
    toastPostion: ToastPosition;
    activePage?: string,
    toastHidden: boolean,
    hideToast: () => void,
    reidReason: string
}

const ReportsLaunchPage: FunctionComponent<ReportLaunchPageProps> = (props) => {
    const { reportConfig, addDeveloperEvent, toastPostion, activePage, toastHidden, hideToast, reidReason } = props;
    const { pseudoColumn } = useParams();
    const [heldPseudosToReid, setHeldPseudosToReid] = useState<string[]>([]);
    const [lastSetOfPseudos, setLastSetOfPseudos] = useState<string[]>([]);
    const [launched, setLaunched] = useState(false);
    const [promptForReid, setPromptForReid] = useState(false);
    const { reidentify, reidentifications, lastPseudo, clearList, isLoading, isAuthorised } = useReidentification(reidReason);
    const { reportBreechThreshold, reportMaxReId } = useFrontendConfiguration();
    const [savedEmbedObject, setSavedEmbedObject] = useState<ReportObject>();
    const [largeNumberConfirmed, setLargeNumberConfirmed] = useState(false);
    const [reidentificationRequestCount, setReidentificationRequestCount] = useState(0);
    const [showGuidance, setShowGuidance] = useState(false);

    const clearHistory = () => {
        setLastSetOfPseudos([]);
        clearList();
    }

    const reIdBulk = () => {
        setPromptForReid(false)
        setLastSetOfPseudos(heldPseudosToReid);
        reidentify(heldPseudosToReid);
        setHeldPseudosToReid([]);
        setLargeNumberConfirmed(true);
    }

    const dataPointEventHandler = useCallback(async (event?: CustomEvent<PBIEvent>) => {

        if (event && event.detail.dataPoints[0] && reportBreechThreshold) {

            // pseudo data could be held in either an identity or value field.
            //const identityValues = ...event.detail.dataPoints.flatMap(x => x.identity);
            const dataFields = [...event.detail.dataPoints.flatMap(x => x.identity), ...event.detail.dataPoints.flatMap(x => x.values)]
            let pseudos: Array<PBIIdentity | PBIValues> = []

            // report developer can provide multiple fields containing pseudo identifiers, so check all columns.
            pseudoColumn?.split(':').forEach(pseudo => {
                pseudos = [...pseudos, ...dataFields.filter((x) => {
                    // column could be either a column or a measure so check both properies.
                    if (x.target.column) {
                        return x.target.column.toLowerCase() == pseudo.toLowerCase()
                    }

                    if (x.target.measure) {
                        return x.target.measure.toLowerCase() == pseudo.toLowerCase()
                    }

                    return false;
                })]
            });

            // no Pseduos selected so can return early.
            if (!pseudos.length) {
                return
            }

            setLaunched(true);
            //the psuedo could be contained in one of the following properties - so normalise all as strings
            const normalisedPseudos = pseudos.map(x => {
                if ((x as PBIIdentity).equals) {
                    return ("" + (x as PBIIdentity).equals)
                }
                return (x as PBIValues).value || (x as PBIValues).formattedValue;
            });

            // somevalues might be commaseperated lists of pseudos so we need to check and extract them to new items/
            let formattedPseudos: string[] = [];
            normalisedPseudos.forEach(maybeCommaSeperated => {
                formattedPseudos = formattedPseudos.concat(maybeCommaSeperated.split(','))
            })

            // we might have been given a set containing duplicates so just remove all but one record;
            const uniquePseudos = formattedPseudos.filter((value, index, array) => array.indexOf(value) === index);

            const uniqueNewPseudosCount = uniquePseudos.filter(up => reidentifications.find(reidRecord => reidRecord.pseudo === up) === undefined).length;

            if (uniqueNewPseudosCount > reportMaxReId) {
                toastInfo(`Cannot reidentify more than ${reportMaxReId} patients in a single request. Select a smaller group of patients to see their NHS Numbers.`);
                return;
            }

            // more than 10 so we prompt the user asking if they intended to re-id large number:
            if (reidentifications.length + uniqueNewPseudosCount >= reportBreechThreshold && !largeNumberConfirmed) {
                //cache them and ask the question.
                setHeldPseudosToReid(uniquePseudos);
                setReidentificationRequestCount(reidentifications.length + uniqueNewPseudosCount);
                setPromptForReid(true)
            } else {
                setLastSetOfPseudos(uniquePseudos);
                await reidentify(uniquePseudos);
            }
        } else {
            addDeveloperEvent({ message: "dataSelected: no datapoints found", eventDetails: event });
        }
    }, [reidentifications, largeNumberConfirmed, reportBreechThreshold, addDeveloperEvent, pseudoColumn, reidentify, reportMaxReId])

    useEffect(() => {
        if (savedEmbedObject) {
            savedEmbedObject.off('dataSelected');
            savedEmbedObject.on('dataSelected', dataPointEventHandler);
        }
    }, [savedEmbedObject, dataPointEventHandler]);

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const bindEvents = (embedObject: any) => {
        setSavedEmbedObject(embedObject);
        embedObject.on('loaded', async () => {
            addDeveloperEvent({ message: "bound events" })

            if (activePage) {
                const pages = await embedObject.getPages();

                const pageToActivate = pages.filter(
                    (page: { displayName: string, visibility: number }) => page.displayName.toLocaleLowerCase() === activePage.toLocaleLowerCase()
                        && page.visibility === 0)

                if (pageToActivate.length !== 1) {
                    addDeveloperEvent({ message: `Cannot find page: ${activePage}`, eventDetails: { detail: { availablePages: pages.map((p: { displayName: string; }) => p.displayName) } } })
                    return;
                }

                embedObject.setPage(pageToActivate[0].name);
            }
        })
    }

    return <>
        <div className="flex-grow-1" style={{ position: "absolute", width: "100%", height: (visualViewport ? visualViewport.height : 500) - 40, inset: "33px 0 0 0" }}>
            {reportConfig.type === 'fake' && <FakeReportPage getEmbeddedComponent={bindEvents} />}
            {reportConfig.type !== 'fake' &&
                <PowerBIEmbed
                    embedConfig={reportConfig as IReportEmbedConfiguration}
                    cssClassName="report-container"
                    getEmbeddedComponent={bindEvents}
                />
            }
            <Modal show={promptForReid} size="lg">
                <Modal.Header><h4>Large number of reidentification requests detected</h4></Modal.Header>
                <Modal.Body>
                    <p>We have identified that you have requested the reidentification of <b>{reidentificationRequestCount}</b> patients this session.</p>
                    <p>Please confirm that you have a legitimate reason to re-identify this number of patients.</p>
                    <Row>
                        <Col>
                            <Button onClick={reIdBulk}>Confirm</Button>
                        </Col>
                        <Col className="d-flex justify-content-end">
                            <Button variant="secondary" onClick={() => { setHeldPseudosToReid([]); setPromptForReid(false); setLastSetOfPseudos([]); setShowGuidance(false); }}>Cancel</Button>
                        </Col>
                    </Row>                    
                    <hr />
                    <p><a href="#" onClick={() => {
                        setShowGuidance(!showGuidance);

                    } }>NHS England may trigger an investigation if the number of requests are excessive, you can read this guidance here.</a></p>

                    {showGuidance && <BreachDetails /> }
                    

                </Modal.Body>
            </Modal>
            <ReportToast
                clearList={clearHistory}
                hidden={toastHidden} hide={hideToast}
                lastSelectedPseudo={lastPseudo}
                lastPseudos={lastSetOfPseudos}
                recordLoading={isLoading}
                position={toastPostion}
                reidentificationLoading={true}
                reidentifications={reidentifications}
                launched={launched}
                isAuthorised={isAuthorised}
            />
            <ToastContainer />
        </div>
    </>
}

export default ReportsLaunchPage


