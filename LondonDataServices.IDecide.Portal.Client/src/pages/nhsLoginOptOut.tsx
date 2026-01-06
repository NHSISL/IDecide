import { useEffect, useState } from "react";
import { Button, Container } from "react-bootstrap";


interface nhsLoginOptOutPageProps {
}

export const NhsLoginOptOutPage = ({ }: nhsLoginOptOutPageProps) => {

    const [familyName, setFamilyName] = useState("");
    const [givenName, setGivenName ] = useState("");
    const [dob, setDob] = useState("");
    const [email, setEmail] = useState("");
    const [phone, setPhone] = useState("");
    const [nhsNo, setNhsNo] = useState("");

    useEffect(() => {
        var d = fetch('/patientinfo').then(d => d.json()).then(r => {
            console.log(r);
            setFamilyName(r.family_name);
            setGivenName(r.given_name);
            setDob(r.birthdate);
            setEmail(r.email);
            setPhone(r.phone_number);
            setNhsNo(r.nhs_number);
        });
        
    },[])
    return (
        <Container>
            <div> Name: {familyName}, {givenName} </div>
            <div> dob: {dob} </div>
            <div> email: {email} </div>
            <div> phone: {phone} </div>
            <div> nhsnumber: {nhsNo} </div>
            <Button onClick={() => {
                fetch('/logout', { method: 'POST' }).then(d => {
                    if (d.ok) {
                        window.location.href = '/';
                    }
                })
            }}>Logout</Button>
        </Container>
    );
};

export default NhsLoginOptOutPage;