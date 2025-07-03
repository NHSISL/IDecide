import { Alert } from "react-bootstrap"
import { Agreement } from "../agreementCatalogue"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faCircleExclamation } from "@fortawesome/free-solid-svg-icons"

export const agreement_v1_0 = () => {
    return {
        type: "UserAgreement",
        version: "1.0",
        text: <div>
            <Alert variant="danger"><FontAwesomeIcon icon={faCircleExclamation} /> By using this service, you acknowledge that a clinical safety assessment has not yet been completed, and you do so at your own risk. It is the responsibility of each user to use the system responsibly and exercise caution when relying on its outputs.</Alert>
            <h5>1. Purpose</h5>
            <p>The re-identification service is designed to assist authorised healthcare professionals in securely re-identifying patients under their direct care and governance for the purpose of providing appropriate medical treatment, care, support, or follow-up.</p>

            <h5>2. User Responsibility</h5>
            <p>When using this service, you confirm that:</p>
            <ul>
                <li>You are a licensed healthcare professional, or an individual authorised under relevant data protection and information governance laws and regulations to access patient information.</li>
                <li>You will only use this service for patients who are directly under your care and governance.</li>
                <li>You will not share the patient information that you have accessed with a third party who does not have authority to see it.</li>
                <li>You acknowledge full responsibility for ensuring that your use of this service complies with all applicable laws, regulations, and ethical standards.</li>
                <li>You will always keep passwords and other applicable access controls safe.</li>
                <li>You will not share passwords with anyone else under any circumstances.</li>
                <li>By using the copy and paste function, you acknowledge that copied content may be stored in your device's clipboard, which could be accessible by other applications; please be mindful of storing sensitive information.</li>
            </ul>

            <h5>3. Authorisation</h5>
            <p>You certify that:</p>
            <p>You have the necessary legal authority, patient consent (if required), and institutional approval to re-identify the individual patient records you have requested.</p>
            <p>Misuse of this service for unauthorised purposes, such as identifying individuals not under your care, is strictly prohibited and may result in penalties, including revocation of access to the service and tumbling the pseudo-key.</p>
            <p>You have been trained in GDPR legislation and are aware that your organisation may be subject to penalties imposed by the ICO (Information Commissioner's Office) if you are found to have breached that legislation.</p>

            <h5>4. Compliance with Data Protection Laws</h5>
            <p>You agree to comply with all relevant data protection and privacy laws, including but not limited to the <strong>UK Data Protection Act 2018, the UK General Data Protection Regulation (UK GDPR)</strong>, or equivalent local regulations.</p>
            <ul>
                <li>You will handle all re-identified data responsibly and securely.</li>
                <li>You will not share or transfer re-identified information to unauthorised third parties.</li>
            </ul>

            <h5>5. Confidentiality</h5>
            <p>You understand that all data accessed through this service is strictly confidential and must only be used for legitimate medical or operational purposes directly related to patient care.</p>

            <h5>6. Accountability</h5>
            <p>You accept full accountability for your actions while using this service. This includes any breach of patient confidentiality or misuse of re-identified data.</p>

            <h5>7. Prohibited Uses</h5>
            <p>You agree not to use this service for:</p>
            <ul>
                <li>Research or analytics without explicit patient consent and/or ethics approval.</li>
                <li>Commercial purposes unrelated to patient care.</li>
                <li>Any activity that breaches applicable ethical, legal, or professional standards.</li>
            </ul>

            <h5>8. Audit and Monitoring</h5>
            <p>The service provider reserves the right to audit usage logs and monitor activity to ensure compliance with these terms and conditions. Unauthorised access or misuse may result in immediate termination of access and legal action.</p>


            <h5>9. Disclaimer</h5>
            <p>The service provider assumes no liability for any misuse of the service or any breach of confidentiality resulting from user actions. Users are solely responsible for ensuring the lawful and ethical use of the service.</p>

            <h5>10. Changes to Terms and Conditions</h5>
            <p>The service provider reserves the right to update or modify these terms and conditions at any time. Continued use of the service constitutes acceptance of any changes.</p>
        </div>
    } as Agreement
}