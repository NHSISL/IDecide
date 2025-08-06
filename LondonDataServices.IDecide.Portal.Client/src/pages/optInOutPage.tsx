import { Container } from "react-bootstrap";
import OptInOptOut from "../components/optInOptOut/optInOptOut";
import { useStep } from "../hooks/useStep";

export const OptInOutPage = () => {
    const { createdPatient } = useStep();
    return (
        <Container style={{ padding: 20 }}>
            <OptInOptOut createdPatient={createdPatient} />
        </Container>
    );
};