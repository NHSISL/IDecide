import React from "react";
import { useTranslation } from "react-i18next";
import { Container, Row, Col } from "react-bootstrap";


export const PatientSearch = () => {
    const { t: translate } = useTranslation();


    return (
        <Container>
            <Row className="custom-col-spacing">
                <Col xs={12} md={6} lg={6}>
                   <h1>GET PATIENT</h1>
                </Col>
                <Col xs={12} md={6} lg={6} className="custom-col-spacing">
                   
                </Col>
            </Row>
        </Container>
    );
};

export default PatientSearch;