import { Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const AccessibilityStatementPage = () => {
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
                    <h1>Accessibility Statement</h1>
                    <p>
                        This accessibility statement applies to the <strong>optout.dataservice.london.nhs.uk</strong> website.
                    </p>
                    <p>
                        This website is run by NHS OneLondon. We want as many people as possible to be able to use this website. For example, that means that you should be able to:
                    </p>
                    <ul>
                        <li>zoom in up to 200% and on all screen sizes (desktop, tablet, mobile)</li>
                        <li>see text buttons &amp; links with an appropriate contrast including a visible focus outline</li>
                        <li>navigate most of the website using just a keyboard</li>
                        <li>listen to most of the website using a screen reader (including the most recent versions of JAWS, NVDA, ChromeVox and VoiceOver)</li>
                    </ul>
                    <p>
                        We’ve also made the website text as simple as possible to understand.
                    </p>
                    <p>
                        <a href="https://mcmw.abilitynet.org.uk/" target="_blank" rel="noopener noreferrer">
                            AbilityNet
                        </a> has advice on making your device easier to use if you have a disability.
                    </p>
                    <h2>Reporting accessibility problems with this website</h2>
                    <p>
                        We’re always looking to improve the accessibility of this website. Contact us if you find any problems with accessing our site.
                    </p>
                    <p>
                        Email: <a href="mailto:NELondonicb.oneLondon.opt-out@nhs.net">NELondonicb.oneLondon.opt-out@nhs.net</a>
                    </p>
                    <p>
                        We'll consider your request and send you a response.
                    </p>
                    <h2>Enforcement procedure</h2>
                    <p>
                        The Equality and Human Rights Commission (EHRC) is responsible for enforcing the Public Sector Bodies (Websites and Mobile Applications) (No. 2) Accessibility Regulations 2018 (the ‘accessibility regulations’). If you’re not happy with how we respond to your complaint, contact the <a href="https://www.equalityadvisoryservice.com/" target="_blank" rel="noopener noreferrer">Equality Advisory and Support Service (EASS)</a>.
                    </p>
                </div>
            </Container>
        </>
    );
};

export default AccessibilityStatementPage;