import { Container } from "react-bootstrap";
import { AppFlow } from "../components/appFlow";
import { useLocation } from "react-router-dom";

export const AppFlowPage = () => {
    const location = useLocation();
    const powerOfAttourney = location.state?.powerOfAttourney === true;

    return (
        <Container style={{ padding: 20 }}>
            <AppFlow powerOfAttourney={powerOfAttourney} />
        </Container>
    );
};