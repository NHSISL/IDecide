import { useNavigate } from "react-router-dom";
import { Button } from "react-bootstrap";

export const Login = () => {
    const navigate = useNavigate();

    return (
        <div>
            Login page
            <Button
                onClick={() => navigate("/optOut", { state: { powerOfAttorney: false } })}
                style={{ margin: "0 0 1rem 1rem", width: 260, fontWeight: 600, minHeight: 75 }}>
                Login
            </Button>
        </div>
    );
};

export default Login;