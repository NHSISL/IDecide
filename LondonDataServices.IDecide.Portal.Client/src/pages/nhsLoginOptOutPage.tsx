import { useEffect, useState } from "react";
import { Button, Container } from "react-bootstrap";

export const NhsLoginOptOutPage = () => {
    const [familyName, setFamilyName] = useState("");
    const [givenName, setGivenName] = useState("");
    const [dob, setDob] = useState("");
    const [email, setEmail] = useState("");
    const [phone, setPhone] = useState("");
    const [nhsNo, setNhsNo] = useState("");

    useEffect(() => {
        fetch("/api/patients/patientInfo")
            .then((response) => response.json())
            .then((data) => {
                setFamilyName(data.family_name ?? "");
                setGivenName(data.given_name ?? "");
                setDob(data.birthdate ?? "");
                setEmail(data.email ?? "");
                setPhone(data.phone_number ?? "");
                setNhsNo(data.nhs_number ?? "");
            })
            .catch(() => {
                // Optionally handle error here
                setFamilyName("");
                setGivenName("");
                setDob("");
                setEmail("");
                setPhone("");
                setNhsNo("");
            });
    }, []);

    const handleLogout = () => {
        fetch("/logout", { method: "POST" }).then((response) => {
            if (response.ok) {
                window.location.href = "/";
            }
        });
    };

    return (
        <Container>
            <div>
                Name: {familyName}, {givenName}
            </div>
            <div>dob: {dob}</div>
            <div>email: {email}</div>
            <div>phone: {phone}</div>
            <div>nhsnumber: {nhsNo}</div>
            <Button onClick={handleLogout}>Logout</Button>
        </Container>
    );
};

export default NhsLoginOptOutPage;