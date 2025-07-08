import React, { FunctionComponent, useState, useRef, useEffect } from "react";
import { Form, Button, Card, Spinner, Alert, OverlayTrigger, Tooltip } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import UserAccessSearch from "../userAccessSearch/userAccessSearch";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { OverlayInjectedProps } from "react-bootstrap/esm/Overlay";
import { useFileChange } from "../../hooks/useFileChange";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";

const CsvReIdentificationDetailCardView: FunctionComponent = () => {

    const [headerColumns, setHeaderColumns] = useState<string[]>([]);
    const [csvData, setCsvData] = useState<string | null>(null);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [selectedHeaderColumnIndex, setSelectedHeaderColumnIndex] = useState<number>(0);
    const [selectedUser, setSelectedUser] = useState<UserAccessView | undefined>();
    const { submit, loading } = reIdentificationService.useRequestReIdentificationCsv();
    const [error, setError] = useState<string[]>([]);
    const [success, setSuccess] = useState("");
    const [savedSuccessfull, setSavedSuccessfull] = useState(false);
    const [fileName, setFileName] = useState<string>("");
    const [hasHeaderRecord, setHasHeaderRecord] = useState<boolean>(false);
    const [reason, setReason] = useState<string>("");
    const account = useMsal();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const { configuration } = useFrontendConfiguration();
    const { handleFileChange } = useFileChange(setError, setFileName, setHeaderColumns, setCsvData, hasHeaderRecord, configuration?.csvMaxReId);

    useEffect(() => {
        if (fileInputRef.current?.files?.[0]) {
            handleFileChange({ target: fileInputRef.current } as React.ChangeEvent<HTMLInputElement>);
        }
    }, [hasHeaderRecord, handleFileChange]);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const acc = account.accounts[0];
        const [firstName, lastName] = acc.name ? acc.name.split(" ") : ["", ""];

        const csvIdentificationRequest: AccessRequest = {
            impersonationContext: undefined,
            identificationRequest: undefined,
            csvIdentificationRequest: {
                id: crypto.randomUUID(),
                requesterUserId: acc.idTokenClaims?.oid,
                requesterFirstName: firstName,
                requesterLastName: lastName,
                requesterDisplayName: acc.name,
                requesterEmail: acc.username,
                requesterJobTitle: " ",
                recipientUserId: selectedUser?.UserId,
                recipientFirstName: selectedUser?.givenName || "",
                recipientLastName: selectedUser?.surname || "",
                recipientDisplayName: selectedUser?.displayName || "",
                recipientEmail: selectedUser?.email || "",
                recipientJobTitle: selectedUser?.jobTitle || "",
                data: csvData || "",
                reason: reason,
                organisation: selectedUser?.orgCode,
                createdBy: acc.username,
                updatedBy: acc.username,
                createdDate: new Date(),
                updatedDate: new Date(),
                filepath: fileName.replace(/\.csv$/i, ""),
                hasHeaderRecord: hasHeaderRecord,
                identifierColumnIndex: selectedHeaderColumnIndex
            }
        }
        setError([]);
        submit(csvIdentificationRequest).then((d) => {
            console.log("Sent", d);
            setSavedSuccessfull(true)
        }).catch((error) => {
            setSavedSuccessfull(false);
            setSuccess("");
            if (error.response && error.response.data && error.response.data.errors) {
                const errorMessages = error.response.data.errors.identifier as string[];
                setError(errorMessages);
            } else {
                setError(["Something went wrong when saving, please contact an administrator"]);
            }
        });
    };

    const handleHeaderColumnChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedColumn = e.target.value;
        setSelectedHeaderColumn(selectedColumn);
        const index = headerColumns.indexOf(selectedColumn);
        setSelectedHeaderColumnIndex(index);

        if (csvData) {
            const decodedData = atob(csvData);
            const rows = decodedData.split("\n");

            if (rows.length > 1) {
                const nextRow = rows[1].split(",");
                const value = nextRow[index];

                if (/^\d+$/.test(value)) {
                    setError([]);
                    setSuccess(`The value "${value}" in the next row at the selected column index is a valid Pseudo Identifier consisting of only digits.`);
                } else {
                    setSuccess("");
                    setError([`The value "${value}" in the next row for the selected column index is not a valid Pseudo Identifier, please follow the guidance in the help section.`]);
                }
            }
        }
    };

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newValue = e.target.checked;
        setHasHeaderRecord(newValue);
        setError([]);
    };

    const renderTooltip = (props: OverlayInjectedProps): React.ReactElement => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to upload a CSV of pseudo identifiers for reidentification.
        </Tooltip>
    );

    return (
        <>
            {!savedSuccessfull ? (
                <>
                    <Card.Header>
                        <Card.Title className="text-start">
                            <OverlayTrigger placement="right" overlay={renderTooltip}>
                                <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                            </OverlayTrigger>&nbsp;Dataset Upload
                        </Card.Title>
                    </Card.Header>

                    <Card.Body>

                        <Card.Subtitle className="text-start text-muted mb-3">
                            <small>
                                Please upload your CSV file below, specify the reason for identifying these patients
                                from the drop-down menu, and select the column containing the identifier.
                            </small>
                        </Card.Subtitle>
                        <Form onSubmit={handleSubmit}>
                            <UserAccessSearch selectUser={(userAccess) => { setSelectedUser(userAccess) }} labelText="Recipient Email Address" />
                            <Form.Group className="text-start">
                                <Form.Label><strong>Upload CSV:</strong></Form.Label>

                                <Form.Check
                                    type="checkbox"
                                    label="Has Header Record"
                                    id="hasHeadRecord"
                                    onChange={handleCheckboxChange} />

                                <div className="d-flex align-items-center">
                                    <Form.Control
                                        type="file"
                                        name="PseudoCsv"
                                        onChange={handleFileChange}
                                        placeholder="Upload CSV"
                                        accept=".csv"
                                        required
                                        ref={fileInputRef} />

                                </div>
                                <Form.Text className="text-muted">
                                    Please upload your CSV (other file types will be rejected).
                                </Form.Text>
                            </Form.Group>
                            <br />
                            {headerColumns.length > 0 && (
                                <>
                                    <Form.Group className="text-start">
                                        <Form.Label><strong>Select Identifier Column From Csv:</strong></Form.Label>
                                        <Form.Select
                                            value={selectedHeaderColumn}
                                            onChange={handleHeaderColumnChange}
                                            required>
                                            <option value="" disabled>Please select a column...</option>
                                            {headerColumns.map((column, index) => (
                                                <option key={index} value={column}>
                                                    {`Col-${index + 1} - ${column}`}
                                                </option>
                                            ))}
                                        </Form.Select>
                                        <Form.Text className="text-muted">
                                            Please choose the correct column for the Pseudo Identifier.
                                        </Form.Text>
                                    </Form.Group>
                                    <br />
                                </>
                            )}
                            <Form.Group className="text-start">
                                <Form.Label><strong>Re-identification Reason:</strong></Form.Label>

                                <Form.Control
                                    as="textarea"
                                    value={reason}
                                    onChange={(e) => setReason(e.target.value)}
                                    placeholder="Enter a reason"
                                    rows={3}
                                    required
                                />

                                <Form.Text className="text-muted">
                                    Please supply a reason why you are requesting to re-identify, this will be visible in the email to the recipient.
                                </Form.Text>
                            </Form.Group>
                            <br />

                            {error && error.length > 0 && (
                                <Alert variant="danger">
                                    Something went wrong, please see details below:
                                    <ul>
                                        {error.map((errMsg, index) => (
                                            <li key={index}>{errMsg}</li>
                                        ))}
                                    </ul>
                                </Alert>
                            )}

                            {success && <Alert variant="success">
                                {success}
                            </Alert>}
                            <Button type="submit" disabled={!selectedHeaderColumn || !selectedUser || !selectedHeaderColumn || !!error.length}>
                                {!loading ? <>Send File</> : <Spinner />}
                            </Button>
                        </Form>
                    </Card.Body>
                </>
            ) : (
                <>
                    <Alert variant="success" className="mb-0">
                        <h4>CSV Sent</h4>
                        <p>The recipient should receive an email with a link to download the re-identified file.</p>
                        <p>Please ensure the recipient checks their inbox and follows the instructions provided in the email to access the file.</p>
                        <p>Alternatively, the recipient can launch their worklist in the portal and re-identify from there.</p>
                        <p><strong>Note:</strong> Files will last for 7 days and then be removed from the system.</p>
                    </Alert>

                </>
            )}
        </>
    );
}

export default CsvReIdentificationDetailCardView;