import { Container } from "react-bootstrap";
import { useParams } from "react-router-dom";
import ImpersonationContextDetailManage from "../components/impersonationContext/impersonationContextDetailManage";

export const ImpersonationManagePage = () => {
    const { impersonationIdentificationRequestId } = useParams<{ impersonationIdentificationRequestId: string }>();

    return (
        <Container fluid>
            <ImpersonationContextDetailManage impersonationIdentificationRequestId={impersonationIdentificationRequestId} />
        </Container>
    )
}