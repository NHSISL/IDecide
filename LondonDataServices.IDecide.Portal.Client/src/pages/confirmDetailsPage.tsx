import { Container } from "react-bootstrap";
import ConfirmDetails from "../components/confirmDetails/confirmDetails";

interface ConfirmDetailsPageProps {
    goToConfirmCode: () => void;
}

export const ConfirmDetailsPage = ({ goToConfirmCode }: ConfirmDetailsPageProps) => {
    return (
        <Container>
            <ConfirmDetails goToConfirmCode={goToConfirmCode} />
        </Container>
    );
};

export default ConfirmDetailsPage;