import { Container, Row, Col } from "react-bootstrap";

export const Home = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <h3>Welcome to the Re-Identification Portal</h3>
                    <br />
                    <p>
                        The One London Re-Identification platform has been purpose-built to support Integrated Care Boards (ICBs),
                        providing secure and efficient re-identification of patient records at scale.
                        Designed to seamlessly integrate with Business Intelligence (BI) tools,
                        the platform empowers analysts and data teams to unlock critical insights while maintaining stringent data protection standards.
                    </p>
                    <p>
                        The platform uses a permission-based model to verify user-patient relationships, cross-checking
                        with patient data from the Personal Demographic Service (PDS). It ensures privacy by redacting
                        patient details when access is not authorised, maintaining compliance and security.
                    </p>

                    <h2>Key Features and Capabilities</h2>
                    <ul>
                        <li><strong>Single Record Re-Identification</strong> - Instantly re-identify individual patient records by submitting a single Pseudo ID.</li>
                        <li><strong>Bulk Dataset Re-Identification</strong> - Upload CSV files containing multiple pseudo codes for batch processing, enabling efficient and large-scale re-identification.</li>
                        <li><strong>Worklist and Download Management</strong> - Track and manage datasets through an intuitive worklist, allowing users to monitor downloads, access histories, and track when and by whom datasets were retrieved.</li>
                        <li><strong>Power BI Integration</strong> - Generate secure report links directly from the platform, streamlining the creation of URLs for seamless Power BI integration and data visualisation.</li>
                        <li><strong>Project and Automation Tools</strong> - A forthcoming project section will allow users to request SAS tokens, facilitating secure connections to automatically generated Azure containers. This will enable automated dataset drop-off and return workflows, further enhancing efficiency and automation.</li>
                    </ul>

                    <p>
                        With a focus on privacy, compliance, and operational efficiency, the One London Re-Identification platform provides healthcare organisations with the tools to manage sensitive patient data responsibly and effectively. Whether re-identifying a single record or managing large datasets, the platform ensures secure, auditable, and scalable data workflows for all users.
                    </p>
                    
                </Col>
            </Row>
            <Row>
                <Col>
                </Col>
            </Row>
        </Container>
    );
}