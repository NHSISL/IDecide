import { Container } from "react-bootstrap";
import { useParams } from "react-router-dom";
import CsvReIdentificationDownloadDetail from "../components/csvReIdentification/csvReIdentificationDownloadDetail";

export const CsvReIdentificationDownloadPage = () => {
    const { csvIdentificationRequestId } = useParams<{ csvIdentificationRequestId: string }>();

    return (
        <Container fluid>
            <CsvReIdentificationDownloadDetail csvIdentificationRequestId={csvIdentificationRequestId} />
        </Container>
    )
}