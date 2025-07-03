import { faCopyright } from '@fortawesome/free-solid-svg-icons/faCopyright';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Col, Container, Row } from "react-bootstrap";
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

const FooterComponent: React.FC = () => {
    const { configuration } = useFrontendConfiguration();
    return (
        <Container>
            <Row className="bg-light">
                <Col className="m-2">
                    <small>
                        <FontAwesomeIcon icon={faCopyright} className="me-2 fa-icon fa-regular" />
                        2025 One London. All rights reserved.
                        <br />
                        <strong>Version:&nbsp;{configuration?.version}</strong>
                    </small>
                </Col>
            </Row>
        </Container>
    );
}

export default FooterComponent;