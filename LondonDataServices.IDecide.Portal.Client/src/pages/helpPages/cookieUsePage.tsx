import React from "react";
import { Container, Card } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const CookiePage = () => {
    const navigate = useNavigate();

    return (
        <Container style={{ padding: 20, maxWidth: 900 }}>
            <button
                className="nhsuk-back-link"
                type="button"
                onClick={() => navigate(-1)}
            >
                <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                Back
            </button>
            <div style={{ marginTop: 24 }}>
                <h1>Cookies on this website</h1>
                <Card className="mb-4">
                    <Card.Body>
                        <h2 className="h5">What cookies are</h2>
                        <p>
                            This website uses cookies. Cookies are small text files that are saved to your device when you use our website. They perform important functions such as remembering preferences and helping us to measure the performance of the website so that we can continue to improve our online services.
                        </p>
                        <p>
                            Our cookies aren’t used to identify you personally. They’re just here to make the site work better for you. You can manage and/or delete these files as you wish.
                        </p>
                    </Card.Body>
                </Card>

                <Card className="mb-4">
                    <Card.Body>
                        <h2 className="h5">Strictly Necessary Cookies</h2>
                        <p>
                            To navigate our website and use all the features and functionality, some strictly necessary cookies are required. The website cannot function without these cookies. You cannot opt out of these.
                        </p>
                        <p>We do not:</p>
                        <ul>
                            <li>share any of the data we collect with others</li>
                            <li>use this data to identify individuals</li>
                            <li>use cookies on this website that collect information for marketing &amp; analytical purposes</li>
                            <li>use cookies on this website that collect information about what other websites you visit (often referred to as privacy intrusive cookies)</li>
                        </ul>
                    </Card.Body>
                </Card>

                <Card>
                    <Card.Body>
                        <h2 className="h5">Managing Cookies</h2>
                        <p>
                            You can prevent the downloading of cookies on your device, block their use or receive notification that cookies are being downloaded by changing the settings on your web browser. However, please be aware that blocking or restricting the use of cookies on this site may prevent you from benefiting from some of the website’s features.
                        </p>
                        <p>
                            To find out more or control and delete cookies as you wish, visit&nbsp;
                            <a href="https://www.allaboutcookies.org/" target="_blank" rel="noopener noreferrer">
                                allaboutcookies.org
                            </a>.
                        </p>
                    </Card.Body>
                </Card>
            </div>
        </Container>
    );
};

export default CookiePage;