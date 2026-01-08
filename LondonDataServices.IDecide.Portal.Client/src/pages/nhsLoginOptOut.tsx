import { useEffect, useState } from "react";
import { Container } from "react-bootstrap";
import ConfirmNhsLoginDetails from "../components/confirmNhsLoginDetails/confirmNhsLoginDetails";

export const NhsLoginOptOutPage = () => {
    
    return (
        <Container>
            <ConfirmNhsLoginDetails />
        </Container>
    );
};

export default NhsLoginOptOutPage;