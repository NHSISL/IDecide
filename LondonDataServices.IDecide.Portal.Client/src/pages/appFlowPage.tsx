import { Container } from "react-bootstrap";
import { AppFlow } from "../components/appFlow";
import { useLocation } from "react-router-dom";

export const AppFlowPage = () => {
    const location = useLocation();
    const powerOfAttorney = location.state?.powerOfAttorney === true;

    return (
        <Container style={{ padding: 20 }}>
            <AppFlow powerOfAttorney={powerOfAttorney} />
        </Container>
    );
};