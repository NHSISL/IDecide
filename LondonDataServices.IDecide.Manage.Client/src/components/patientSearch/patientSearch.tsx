import React, { useState } from "react";
import { Container, Row, Col, Form, Button, Alert, Table, Spinner, Card } from "react-bootstrap";
import { patientViewService } from "../../services/views/patientViewService";
import { decisionViewService } from "../../services/views/decisionViewService";
import { useNavigate } from "react-router-dom";
import moment from "moment";
import { ConsumerAdoption } from "../../models/consumerAdoptions/consumerAdoption";

export const PatientSearch = () => {
    const navigate = useNavigate();
    const [searchMode, setSearchMode] = useState<"nhs" | "details">("nhs");
    const [nhsNumber, setNhsNumber] = useState("");
    const [submittedNhsNumber, setSubmittedNhsNumber] = useState<string | undefined>(undefined);
    const [details, setDetails] = useState({ surname: "", postcode: "", dobDay: "", dobMonth: "", dobYear: "" });
    //const [selectedDecisionId, setSelectedDecisionId] = useState<string | undefined>(undefined);

    const [submittedDetails, setSubmittedDetails] = useState<{
        surname: string;
        postcode: string;
        dobDay: string;
        dobMonth: string;
        dobYear: string;
    } | undefined>(undefined);

    const [lastSubmittedMode, setLastSubmittedMode] = useState<"nhs" | "details" | undefined>(undefined);
    const [error, setError] = useState("");

    const {
        mappedPatients: patientsRetrieved,
        isLoading
    } = patientViewService.useGetAllPatients(
        searchMode === "nhs" && submittedNhsNumber
            ? { nhsNumber: submittedNhsNumber }
            : searchMode === "details" && submittedDetails
                ? {
                    surname: submittedDetails.surname,
                    postCode: submittedDetails.postcode,
                    dateOfBirth: `${submittedDetails.dobYear}-${submittedDetails.dobMonth.padStart(2, "0")}-${submittedDetails.dobDay.padStart(2, "0")}`
                }
                : undefined
    );

    const firstPatientId = patientsRetrieved && patientsRetrieved.length > 0 ? patientsRetrieved[0].id : undefined;

    const {
        mappedDecisions: decisionsByPatientId,
        isLoading: isLoadingDecision
    } = decisionViewService.useGetAllDecisions(firstPatientId ?? "");

    const handleModeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSearchMode(event.target.value as "nhs" | "details");
        setError("");
        setSubmittedNhsNumber(undefined);
        setSubmittedDetails(undefined);
        setLastSubmittedMode(undefined);
    };

    const handleDetailsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setDetails({
            ...details,
            [event.target.name]: event.target.value
        });
        setError("");
    };

    const validateNhsNumber = (value: string) => /^\d{10}$/.test(value);

    const validateDetails = () => {
        if (!details.surname || !details.postcode || !details.dobDay || !details.dobMonth || !details.dobYear) {
            setError("All fields are required.");
            return false;
        }
        if (
            !/^\d{1,2}$/.test(details.dobDay) ||
            !/^\d{1,2}$/.test(details.dobMonth) ||
            !/^\d{4}$/.test(details.dobYear)
        ) {
            setError("Date of birth must be valid.");
            return false;
        }
        setError("");
        return true;
    };

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        if (searchMode === "nhs") {
            if (!validateNhsNumber(nhsNumber)) {
                setError("NHS Number must be 10 digits.");
                return;
            }
            setError("");
            setSubmittedNhsNumber(nhsNumber);
            setSubmittedDetails(undefined);
            setLastSubmittedMode("nhs");
        } else {
            if (!validateDetails()) return;
            setSubmittedNhsNumber(undefined);
            setSubmittedDetails(details);
            setLastSubmittedMode("details");
        }
    };

    return (
        <Container fluid>
            <Row>


                <Col xs={12} md={12} lg={12}>
                    <Form onSubmit={handleSubmit}>
                        <Form.Group>
                            <Form.Check
                                type="radio"
                                label="Search by NHS Number"
                                value="nhs"
                                checked={searchMode === "nhs"}
                                onChange={handleModeChange}
                                inline
                            />
                            <br />
                            <Form.Check
                                type="radio"
                                label="Search by Patient Details"
                                value="details"
                                checked={searchMode === "details"}
                                onChange={handleModeChange}
                                inline
                            />
                            <br /><br />
                        </Form.Group>
                        {searchMode === "nhs" ? (
                            <Form.Group controlId="nhsNumber">
                                <Form.Label>NHS Number</Form.Label>
                                <Form.Control
                                    type="text"
                                    value={nhsNumber}
                                    onChange={e => setNhsNumber(e.target.value.replace(/\D/g, "").slice(0, 10))}
                                    maxLength={10}
                                    required
                                />
                            </Form.Group>
                        ) : (
                            <>
                                <Form.Group controlId="surname">
                                    <Form.Label>Surname</Form.Label>
                                    <Form.Control
                                        type="text"
                                        name="surname"
                                        value={details.surname}
                                        onChange={handleDetailsChange}
                                        required
                                    />
                                </Form.Group>
                                <Form.Group controlId="postcode">
                                    <Form.Label>Postcode</Form.Label>
                                    <Form.Control
                                        type="text"
                                        name="postcode"
                                        value={details.postcode}
                                        onChange={handleDetailsChange}
                                        required
                                    />
                                </Form.Group>
                                <Form.Group>
                                    <Form.Label>Date of Birth</Form.Label>
                                    <div style={{ display: "flex", gap: "0.5rem" }}>
                                        <Form.Control
                                            type="text"
                                            name="dobDay"
                                            placeholder="DD"
                                            value={details.dobDay}
                                            onChange={handleDetailsChange}
                                            maxLength={2}
                                            style={{ width: "4em" }}
                                            required
                                        />
                                        <Form.Control
                                            type="text"
                                            name="dobMonth"
                                            placeholder="MM"
                                            value={details.dobMonth}
                                            onChange={handleDetailsChange}
                                            maxLength={2}
                                            style={{ width: "4em" }}
                                            required
                                        />
                                        <Form.Control
                                            type="text"
                                            name="dobYear"
                                            placeholder="YYYY"
                                            value={details.dobYear}
                                            onChange={handleDetailsChange}
                                            maxLength={4}
                                            style={{ width: "6em" }}
                                            required
                                        />
                                    </div>
                                </Form.Group>
                            </>
                        )}
                        {error && <Alert variant="danger" className="mt-3">{error}</Alert>}
                        <Button variant="primary" type="submit" className="mt-3">
                            Search
                        </Button>
                    </Form>
                </Col>
            </Row>
            {(searchMode === "nhs" && submittedNhsNumber) || (searchMode === "details" && submittedDetails) ? (
                <Row className="mt-4">
                    <Col xs={12} md={12}>
                        {isLoading ? (
                            <Spinner animation="border" />
                        ) : (
                            patientsRetrieved && patientsRetrieved.length > 0 ? (
                                <Card>
                                    <Card.Header>Patient Details</Card.Header>
                                    <Card.Body>
                                        <Table striped bordered hover className="mb-4 w-100">
                                            <tbody>
                                                <tr><th>NHS Number</th><td>{patientsRetrieved[0].nhsNumber}</td></tr>
                                                <tr><th>Title</th><td>{patientsRetrieved[0].title}</td></tr>
                                                <tr><th>Given Name</th><td>{patientsRetrieved[0].givenName}</td></tr>
                                                <tr><th>Surname</th><td>{patientsRetrieved[0].surname}</td></tr>
                                                <tr>
                                                    <th>Date of Birth</th>
                                                    <td>
                                                        {moment(patientsRetrieved[0].dateOfBirth?.toString()).format("Do-MMM-yyyy")}
                                                    </td>
                                                </tr>
                                                <tr><th>Gender</th><td>{patientsRetrieved[0].gender}</td></tr>
                                                <tr><th>Email</th><td>{patientsRetrieved[0].email}</td></tr>
                                                <tr><th>Phone</th><td>{patientsRetrieved[0].phone}</td></tr>
                                                <tr><th>Address</th><td>{patientsRetrieved[0].address}</td></tr>
                                                <tr><th>Post Code</th><td>{patientsRetrieved[0].postCode}</td></tr>
                                                <tr><th>Validation Code</th><td>{patientsRetrieved[0].validationCode}</td></tr>
                                                <tr><th>Validation Code Expires On</th>
                                                    <td>{moment(patientsRetrieved[0].validationCodeExpiresOn?.toString()).format("Do-MMM-yyyy")}</td>
                                                </tr>
                                                <tr><th>Validation Code Matched On</th>
                                                    <td>{moment(patientsRetrieved[0].validationCodeMatchedOn?.toString()).format("Do-MMM-yyyy")}</td>
                                                </tr>
                                                <tr><th>Retry Count</th><td>{patientsRetrieved[0].retryCount}</td></tr>
                                                <tr><th>Notification Preference</th><td>{patientsRetrieved[0].notificationPreference}</td></tr>
                                            </tbody>
                                        </Table>
                                    </Card.Body>
                                </Card>
                            ) : (
                                <>
                                    {patientsRetrieved && patientsRetrieved.length === 0 && lastSubmittedMode === "nhs" && (
                                        <div>
                                            No patients found.{" "}
                                            <a
                                                href="#"
                                                onClick={e => {
                                                    e.preventDefault();
                                                    navigate("/nhsNumberSearch");
                                                }}
                                            >
                                                To search PDS and add patient, click this link here.
                                            </a>
                                        </div>
                                    )}
                                    {patientsRetrieved && patientsRetrieved.length === 0 && lastSubmittedMode === "details" && (
                                        <div>
                                            No patients found.{" "}
                                            <a
                                                href="#"
                                                onClick={e => {
                                                    e.preventDefault();
                                                    navigate("/patientDetailsSearch");
                                                }}
                                            >
                                                To search PDS and add patient, click this link here.
                                            </a>
                                        </div>
                                    )}
                                </>
                            )
                        )}


                        {isLoadingDecision ? (
                            <Spinner animation="border" />
                        ) : (
                            decisionsByPatientId && decisionsByPatientId.length > 0 ? (
                                <Card className="mt-4">
                                    <Card.Header>Decision Details</Card.Header>
                                    <Card.Body>
                                        <Table striped bordered hover className="mb-4 w-100">
                                            <thead>
                                                <tr>
                                                    <th>Decision Type</th>
                                                    <th>Choice</th>
                                                    <th>Created</th>
                                                    <th>Updated</th>
                                                    <th>Resp Person Name</th>
                                                    <th>Resp Person Relationship</th>
                                                    <th>Adoption Date</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {decisionsByPatientId.map((decision, dIdx) => (
                                                    <tr key={dIdx}>
                                                        <td>{decision.decisionType?.name}</td>
                                                        <td>{decision.decisionChoice}</td>
                                                        <td>{decision.createdDate ? moment(decision.createdDate.toString()).format("Do-MMM-yyyy HH:mm") : ""}</td>
                                                        <td>{decision.updatedDate ? moment(decision.updatedDate.toString()).format("Do-MMM-yyyy HH:mm") : ""}</td>
                                                        <td>{decision.responsiblePersonGivenName} {decision.responsiblePersonSurname}</td>
                                                        <td>{decision.responsiblePersonRelationship}</td>
                                                        <td>
                                                            {Array.isArray(decision.consumerAdoptions)
                                                                ? decision.consumerAdoptions
                                                                    .map((adoption: ConsumerAdoption) =>
                                                                        adoption.adoptionDate
                                                                            ? moment(adoption.adoptionDate).format("DD-MM-YYYY HH:mm")
                                                                            : ""
                                                                    )
                                                                    .join(", ")
                                                                : ""}
                                                        </td>
                                                        {/*<td>*/}
                                                        {/*    <Button*/}
                                                        {/*    variant="link"*/}
                                                        {/*    onClick={() => setSelectedDecisionId(decision.id)}*/}
                                                        {/*    style={{ padding: 0 }}*/}
                                                        {/*>*/}
                                                        {/*    Click*/}
                                                        {/*    </Button>*/}
                                                        {/*</td>*/}
                                                    </tr>
                                                ))}
                                            </tbody>
                                        </Table>
                                            {/*<ConsumerAdoptionTable decisionId={selectedDecisionId} />*/}
                                    </Card.Body>
                                </Card>
                            ) : (
                                <>
                                    <br />
                                    <Alert>
                                        <div>No decisions recorded for this patient.</div>
                                    </Alert>
                                </>
                            )
                        )}
                    </Col>
                </Row>
            ) : null}
        </Container>
    );
};

export default PatientSearch;