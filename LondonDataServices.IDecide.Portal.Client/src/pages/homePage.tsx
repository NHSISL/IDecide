import { Container } from "react-bootstrap";
import Home from "../components/home/home";

export const HomePage = () => {
    return (
        <Container fluid>
            <div>
                <div>
                    <Home />
                </div>
            </div>
        </Container>
    );
};

export default HomePage;