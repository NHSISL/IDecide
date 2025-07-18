import { Fieldset, Radios } from "nhsuk-react-components";
import React from "react";
import { Container, Row } from "react-bootstrap";
import { useStep } from "../context/stepContext"; // adjust this path as needed

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';

const steps = ["nhs", "details", "confirm", "choice", "notify"];

const stepLabels: Record<string, string> = {
    nhs: "Provide Your NHS Number",
    details: "Confirm Your Details",
    confirm: "Positive Confirmation",
    choice: "Make Your Choice",
    notify: "Receive Notifications",
};

const stepContent: Record<string, React.ReactNode> = {
    nhs: <p>Your NHS number helps us locate your record quickly and accurately.</p>,
    details: <p>Make sure your name and date of birth are correct before proceeding.</p>,
    confirm: <p>Confirmation means you've reviewed and validated the information provided.</p>,
    choice: <p>Please indicate your preference regarding the opt-out.</p>,
    notify: <p>Choose how you would like to receive updates about your preferences.</p>,
};

const LeftProgress: React.FC = () => {
    const { currentStepIndex, setCurrentStepIndex } = useStep();

    const handleChange = (idx: number) => {
        if (idx === currentStepIndex || idx === currentStepIndex + 1) {
            setCurrentStepIndex(idx);
        }
    };

    const isCurrentStep = (idx: number) => idx === currentStepIndex;
    const isPreviousStep = (idx: number) => idx < currentStepIndex;

    return (
        <Container>
            <Row>
                <form style={{ padding: 20 }}>
                    <div className="leftProgressPadding">
                        <Fieldset.Legend>
                            <h2>The Opt-Out Process</h2>
                        </Fieldset.Legend>

                        <Radios>
                            {steps.map((step, idx) => (
                                <React.Fragment key={step}>
                                    {isPreviousStep(idx) ? (
                                        <div
                                            className="completed-tick"
                                            aria-label={`${stepLabels[step]} completed`}
                                            role="radio"
                                            aria-checked="true"
                                            tabIndex={-1}
                                            style={{
                                                display: "flex",
                                                alignItems: "center",
                                                cursor: "default",
                                                marginBottom: 8,
                                                color: "black",
                                            }}
                                        >
                                            <FontAwesomeIcon
                                                icon={faCheckCircle}
                                                style={{ marginRight: 8, fontSize: '2.5rem', color: "#006435" }}
                                                aria-hidden="true"
                                            />
                                            <span>{stepLabels[step]}</span>
                                        </div>
                                    ) : (
                                        <Radios.Radio
                                            id={`radio-${step}`}
                                            name="step"
                                            value={step}
                                            disabled={true} // <-- Always disabled
                                            checked={isCurrentStep(idx)}
                                            onChange={() => { }} // No-op
                                            style={{ marginBottom: 8 }}
                                        >
                                            <span className="radio-label">{stepLabels[step]}</span>
                                        </Radios.Radio>
                                    )}

                                    {isCurrentStep(idx) && (
                                        <div className="nhsuk-radios__conditional">{stepContent[step]}</div>
                                    )}
                                </React.Fragment>
                            ))}
                        </Radios>
                    </div>
                </form>
            </Row>
        </Container>
    );
};

export default LeftProgress;
