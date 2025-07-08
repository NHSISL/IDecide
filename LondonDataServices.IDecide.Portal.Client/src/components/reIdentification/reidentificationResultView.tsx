import { FunctionComponent, ReactElement } from "react";
import { ReIdRecord } from "../../types/ReIdRecord"
import { Alert, Card, CardFooter } from "react-bootstrap";
import CopyIcon from "../core/copyIcon";
import { useTimer } from "../../hooks/useTimer";
import { UserProfile } from "../securitys/userProfile";

type ReidentificationResultViewProps = {
    reidentificationRecord: ReIdRecord;
    children?: ReactElement
}
const ReidentificationResultView: FunctionComponent<ReidentificationResultViewProps> = ({ reidentificationRecord, children }) => {

    const { remainingSeconds, timerExpired } = useTimer(60);

    if (reidentificationRecord.hasAccess) {
        return <>
            <Card>
                <Card.Body>
                    {!timerExpired && <>
                        <Alert variant="success">
                            NHS Number: {reidentificationRecord.nhsnumber}&nbsp;<CopyIcon content={reidentificationRecord.nhsnumber} />
                        </Alert>
                        <small>Hiding in: {remainingSeconds}</small>
                    </>
                    }
                    {timerExpired && <Alert variant="success">
                        Patient NHS Number hidden to maintain confidentiality.
                    </Alert>
                    }
                </Card.Body>
                {children &&
                    <CardFooter>
                        {children}
                    </CardFooter>
                }
            </Card>
        </>
    }

    return <Card>
        <Card.Header><h4>Re-identification is not permitted.</h4></Card.Header>
        <Card.Body>
            <Alert variant="danger">
                <p>
                    It appears you tried to re-identify a patient you don't have access to.
                </p>
                <p>
                    Please check that the
                    patient is registered with a GP practice you're authorised to access. To see your ODS organisations
                    in the re-identification tool <u><UserProfile modalTitle="Click Here" className="customAnchor" /></u> to view your user profile in.
                    For more access, contact your local ICB.

                </p>
                <p>
                    <strong>Note</strong> that any
                    changes to the patient's registration may take up to 24 hours to update in the system.
                </p>
            </Alert>
        </Card.Body>
        {children && <CardFooter>
            {children}
        </CardFooter>
        }
    </Card>
}

export default ReidentificationResultView;