import { Container } from "react-bootstrap";
import Home from "../components/home/home";

export const HomePage = () => {
    return (
        <Container fluid>
            <div className="fullscreen-bg">
                <div className="fullscreen-bg-overlay">
                    <Home />
                </div>
            </div>
        </Container>
    );
};

export default HomePage;