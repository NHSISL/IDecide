import { FunctionComponent, useState } from "react";
import { Container, Row, Alert } from "react-bootstrap";
import CsvReIdentificationDetailCard from "./csvReIdentificationDetailCard";
import { BreachModal } from "../breachDetails/BreachModal";

const CsvReIdentificationDetail: FunctionComponent = () => {
    const [showBreachModal, setShowBreachModal] = useState(false);

    return (
        <>
            <Container className="">
                <Row className="justify-content-md-center">
                    <CsvReIdentificationDetailCard />
                </Row>
            </Container>
            <br />
            <Container className="">
                <Row className="justify-content-md-center">
                    <Alert variant="secondary" style={{ width: '50rem' }}>
                        <p>This page provides a simple re-identification for multi-patient pseudo identifying.</p>
                        <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation.</p>
                        <p><strong>Note:</strong> all re-identification requests are subject to breach monitoring and reporting</p>
                        <p>Details of breach thresholds can be found <a href='#' onClick={() => setShowBreachModal(true)}>here</a>.</p>
                    </Alert>
                </Row>
            </Container>

            <BreachModal show={showBreachModal} hide={() => setShowBreachModal(false)} />
        </>
    );
};

export default CsvReIdentificationDetail;