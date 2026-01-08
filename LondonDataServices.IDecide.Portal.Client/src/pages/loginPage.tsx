import { Container } from "react-bootstrap";
import Login from "../components/login/login";

export const LoginPage = () => {
    return (
        <Container fluid>
            <div>
                <Login />
            </div>
        </Container>
    );
};

export default LoginPage;