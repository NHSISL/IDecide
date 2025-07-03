import { Container, Row, Col } from "react-bootstrap";

export const Home = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <h3>Welcome to Re-Identification Management Portal</h3>
                    <br />
                    <p>
                        The One London Re-Identification Management platform empowers administrators and data governance teams to manage user access, configure lookup tables, and oversee audit logs for the One London re-identification ecosystem.
                    </p>
                    <p>This secure management interface provides the tools to:</p>
                    <ul>
                        <li><strong>Control User Access</strong> - Assign, modify, and revoke permissions, ensuring the right individuals have appropriate levels of access.</li>
                        <li><strong>Configure Lookups</strong> - Maintain lookup tables and reference data, streamlining integration with patient datasets.</li>
                        <li><strong>Audit and Monitor</strong> - View and manage comprehensive audit logs, tracking user actions and system events to ensure accountability and compliance.</li>
                    </ul>
                    <p>
                        Built to uphold rigorous security and privacy standards, the platform facilitates seamless integration with BI tools and aligns with ICB policies, ensuring patient data remains protected and re-identification processes are transparent and controlled.
                    </p>
                </Col>
            </Row>
        </Container>
    );
}