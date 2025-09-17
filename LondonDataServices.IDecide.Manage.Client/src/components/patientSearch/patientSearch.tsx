import React, { useState } from "react";
import { Container, Row, Col, Form, Button, Alert, Table, Spinner } from "react-bootstrap";
import { patientViewService } from "../../services/views/patientViewService";
import { useNavigate } from "react-router-dom";
import moment from "moment";

export const PatientSearch = () => {
    const navigate = useNavigate();
    const [searchMode, setSearchMode] = useState<"nhs" | "details">("nhs");
    const [nhsNumber, setNhsNumber] = useState("1234567890");
    const [submittedNhsNumber, setSubmittedNhsNumber] = useState<string | undefined>(undefined);
    const [details, setDetails] = useState({
        surname: "Smith",
        postcode: "LS18 9ZZ",
        dobDay: "03",
        dobMonth: "01",
        dobYear: "2010"
    });
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
                <Col xs={12} md={8} lg={6}>
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
                    {(searchMode === "nhs" && submittedNhsNumber) || (searchMode === "details" && submittedDetails) ? (
                        <div className="mt-4">
                            {isLoading ? (
                                <Spinner animation="border" />
                            ) : (
                                patientsRetrieved && patientsRetrieved.length > 0 ? (
                                    patientsRetrieved.map((patient, idx) => (
                                        <Table striped bordered hover key={idx} className="mb-4 w-100">
                                            <tbody>
                                                <tr><th>NHS Number</th><td>{patient.nhsNumber}</td></tr>
                                                <tr><th>Title</th><td>{patient.title}</td></tr>
                                                <tr><th>Given Name</th><td>{patient.givenName}</td></tr>
                                                <tr><th>Surname</th><td>{patient.surname}</td></tr>
                                                <tr>
                                                    <th>Date of Birth</th>
                                                    <td>
                                                        {moment(patient.dateOfBirth?.toString()).format("Do-MMM-yyyy")}
                                                    </td>
                                                </tr>
                                                <tr><th>Gender</th><td>{patient.gender}</td></tr>
                                                <tr><th>Email</th><td>{patient.email}</td></tr>
                                                <tr><th>Phone</th><td>{patient.phone}</td></tr>
                                                <tr><th>Address</th><td>{patient.address}</td></tr>
                                                <tr><th>Post Code</th><td>{patient.postCode}</td></tr>
                                                <tr><th>Validation Code</th><td>{patient.validationCode}</td></tr>
                                                <tr><th>Validation Code Expires On</th>
                                                    <td>{moment(patient.validationCodeExpiresOn?.toString()).format("Do-MMM-yyyy")}</td>
                                                </tr>
                                                <tr><th>Validation Code Matched On</th>
                                                    <td>{moment(patient.validationCodeMatchedOn?.toString()).format("Do-MMM-yyyy")}</td>
                                                </tr>
                                                <tr><th>Retry Count</th><td>{patient.retryCount}</td></tr>
                                                <tr><th>Notification Preference</th><td>{patient.notificationPreference}</td></tr>
                                            </tbody>
                                        </Table>
                                    ))
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
                        </div>
                    ) : null}
                </Col>
            </Row>
        </Container>
    );
};

export default PatientSearch;