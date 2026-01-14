import { Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import HeaderComponent from "../../components/layouts/header";

const ContactPage = () => {
    const navigate = useNavigate();

    return (
        <>
            <Container style={{ padding: 20, maxWidth: 800 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                    aria-label="Go back"
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    Back
                </button>
                <div style={{ marginTop: 24 }}>
                    <h1>Contact Us</h1>
                    <p>
                        The team that operate this website and the Opt-out service are based in Stratford, North East London.
                        In order to contact us you can:
                    </p>
                    <ul>
                        <li>
                            <strong>E-mail Us:</strong>{" "}
                            <a href="mailto:NELondonicb.oneLondon.opt-out@nhs.net">
                                NELondonicb.oneLondon.opt-out@nhs.net
                            </a>
                        </li>
                        <li>
                            <strong>Leave a Voicemail:</strong> 020 3182 2900
                        </li>
                        <li>
                            <strong>Write to us:</strong>
                            <address style={{ marginTop: 8, marginBottom: 0 }}>
                                OneLondon Service Desk<br />
                                North East London ICB<br />
                                10 Vicarage Lane<br />
                                London<br />
                                E15 4ES
                            </address>
                        </li>
                    </ul>
                </div>
            </Container>
        </>
    );
};

export default ContactPage;