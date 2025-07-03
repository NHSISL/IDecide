import { faCheck, faCopy } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react"
import { Alert, Button, Card, Form } from "react-bootstrap"

const ReportHelper: FunctionComponent = () => {
    const [reportGroupId, setReportGroupId] = useState("");
    const [reportId, setReportId] = useState("");
    const [psuedoColumn, setPsuedoColumn] = useState("");
    const [reportPage, setReportPage] = useState("");
    const [url, setUrl] = useState("");
    const [hasCopied, setHasCopied] = useState(false)

    function generateUrl(): void {
        let url = `${window.location.href}/${reportGroupId}/${reportId}/${psuedoColumn}`;
        if(reportPage){
            url += `/${reportPage}`;
        }
        setUrl(url);
    }

    function copyUrl(): void {
        navigator.clipboard.writeText(url);
        setHasCopied(true);
        setTimeout(() => { setHasCopied(false)}, 2000);
    }

    return (<>
        <Card>
            <Card.Header>Report URL Generator</Card.Header>
            <Card.Body>
                <Alert variant="success">Use this tool to help you construct the URL for the re-identification server.</Alert>
                <Form>
                    <Form.Group>
                        <Form.Label>Report Group Id:</Form.Label>
                        <Form.Control value={reportGroupId} onChange={(e) => { setReportGroupId(e.target.value) }} placeholder="Report Group Id" />
                    </Form.Group>
                    <br />
                    <Form.Group>
                        <Form.Label>Report Id:</Form.Label>
                        <br />
                        <Form.Control value={reportId} onChange={(e) => { setReportId(e.target.value) }} placeholder="Report Id" />
                    </Form.Group>
                    <br />
                    <Form.Group>
                        <Form.Label>Column Containing Pseudo Identifer</Form.Label>
                        <br />
                        <Form.Control value={psuedoColumn} onChange={(e) => { setPsuedoColumn(e.target.value) }} placeholder="Column Name with Pseudo" />
                    </Form.Group>
                    <br />
                    <Form.Group>
                        <Form.Label>Report Page to launch </Form.Label> <br />
                        <Form.Control value={reportPage} onChange={(e) => { setReportPage(e.target.value) }} placeholder="Report Page to Launch" />
                        <small>(Leave blank to launch the home page of the report)</small>
                    </Form.Group>
                    <br />
                    <Button onClick={generateUrl}>Generate</Button>
                    <br/>
                    {url && <div>URL: {url} <FontAwesomeIcon icon={hasCopied ? faCheck : faCopy} onClick={copyUrl} /></div>}
                </Form>
            </Card.Body>
        </Card>
    </>)
}

export default ReportHelper;