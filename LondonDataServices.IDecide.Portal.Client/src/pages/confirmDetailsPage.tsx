import { Container } from "react-bootstrap";
import ConfirmDetails from "../components/confirmDetails/confirmDetails";

export const ConfirmDetailsPage = ({ goToConfirmCode }) => {
    return (
        <Container>
            <ConfirmDetails goToConfirmCode={goToConfirmCode} />
        </Container>
    );
};

export default ConfirmDetailsPage;