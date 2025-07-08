import { FunctionComponent, useState } from "react";
import { Alert, Button, Card, Container, Form, FormGroup, Row, Spinner } from "react-bootstrap";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { BreachModal } from "../breachDetails/BreachModal";

type ReportReasonPageProps = {
    setReidReason: (reason: string) => void;
    launchReport: () => void;
    reidReason: string;
    launchError: string;
};

const ReportsReasonPage: FunctionComponent<ReportReasonPageProps> = (props: ReportReasonPageProps) => {
    const { setReidReason, launchReport, reidReason, launchError } = props;
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [showBreachModal, setShowBreachModal] = useState(false);

    return (
        <>
            <Container>
                <Row className="justify-content-md-center">
                    <Card style={{ width: '50rem' }}>
                        <Card.Header>
                            <Card.Link href="/" style={{ color: "black", textDecoration: "none" }}>ISL Re-Identification Portal</Card.Link>
                        </Card.Header>
                        <Card.Body>
                            <div>
                                <h4>Welcome to the One London Re-Identification Portal</h4>
                                <p>The link you have followed provides Re-Identification integrated into Power BI reports.</p>
                                <p>The <a href="/">Re-Identification home page</a> has additional Re-Identification capabilities.</p>
                            </div>
                            <Form className="mt-2">
                                <FormGroup>
                                    <Form.Label>Please select the reason why you are identifying the patients on this report:</Form.Label>
                                    {isLoading ? <div><Spinner /></div> :
                                        <Form.Select onChange={(e) => { setReidReason(e.target.value) }}>
                                            <option value="">--- Select Reason ---</option>
                                            {mappedLookups.map(l => <option key={l.id} value={l.name}>{l.name}</option>)}
                                        </Form.Select>
                                    }
                                </FormGroup>
                                <br />
                                <Button onClick={launchReport} disabled={!reidReason}>Launch Report</Button>
                            </Form>
                            {launchError && <Alert variant="danger" className="mt-2"><code style={{ display: 'block', whiteSpace: 'pre-wrap',  fontFamily: 'arial' }}>{launchError}</code></Alert>}
                            <Alert variant="secondary" className="mt-2">
                                <p><strong>Note:</strong> you will only be able to re-identify patients that are present within your organisation.</p>
                                <p><strong>Note:</strong> all re-identification requests are subject to breach monitoring and reporting</p>
                                <p>Details of breach thresholds can be found  <a href='#' onClick={() => setShowBreachModal(true)}>here</a>.</p>
                            </Alert>
                        </Card.Body>
                    </Card>
                </Row>
            </Container>
            <BreachModal show={showBreachModal} hide={() => setShowBreachModal(false)} />
        </>
    );
}

export default ReportsReasonPage;