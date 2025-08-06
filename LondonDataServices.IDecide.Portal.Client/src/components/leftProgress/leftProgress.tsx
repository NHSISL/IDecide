import { Fieldset, Radios } from "nhsuk-react-components";
import React from "react";
import { Container, Row } from "react-bootstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';

const steps = ["nhs", "details", "confirm", "choice", "notify", "thanks"];

const stepLabels: Record<string, string> = {
    nhs: "Provide Your NHS Number",
    details: "Confirm Your Details",
    confirm: "Positive Confirmation",
    choice: "Make Your Choice",
    notify: "Receive Notifications",
    thanks: "Complete",
};

const stepContent: Record<string, React.ReactNode> = {
    nhs: <p>Your NHS number helps us locate your record quickly and accurately.</p>,
    details: <p>Make sure your name and date of birth are correct before proceeding.</p>,
    confirm: <p>Confirmation means you've reviewed and validated the information provided.</p>,
    choice: <p>Please indicate your preference regarding the opt-out.</p>,
    notify: <p>Choose how you would like to receive updates about your preferences.</p>,
    thanks: <p>Process Complete</p>
};

function useIsMobile() {
    const [isMobile, setIsMobile] = React.useState(false);
    React.useEffect(() => {
        const check = () => setIsMobile(window.innerWidth <= 768);
        check();
        window.addEventListener("resize", check);
        return () => window.removeEventListener("resize", check);
    }, []);
    return isMobile;
}

interface LeftProgressProps {
    currentStepIndex: number;
    setCurrentStepIndex: React.Dispatch<React.SetStateAction<number>>;
}

const LeftProgress: React.FC<LeftProgressProps> = ({ currentStepIndex, setCurrentStepIndex }) => {
    const isMobile = useIsMobile();

    const isCurrentStep = (idx: number) => idx === currentStepIndex;
    const isPreviousStep = (idx: number) => idx < currentStepIndex;
    const isLastStep = (idx: number) => idx === steps.length - 1;

    return (
        <Container>
            <Row>
                <form style={{ padding: 20 }}>
                    <div className="leftProgressPadding">
                        <Fieldset.Legend>
                            <h2>The Opt-Out Process</h2>
                        </Fieldset.Legend>

                        {isMobile ? (
                            <>
                                <div
                                    className="horizontal-stepper"
                                    style={{
                                        display: "flex",
                                        justifyContent: "space-between",
                                        marginBottom: 16,
                                        overflowX: "auto"
                                    }}
                                >
                                    {steps.map((step, idx) => (
                                        <div
                                            key={step}
                                            style={{
                                                flex: 1,
                                                textAlign: "center",
                                                minWidth: 40,
                                                opacity: isPreviousStep(idx) ? 0.5 : 1,
                                                borderBottom: isCurrentStep(idx) ? "3px solid #005eb8" : "1px solid #ccc",
                                                padding: "8px 0",
                                                cursor: isPreviousStep(idx) ? "pointer" : "default"
                                            }}
                                            onClick={() => isPreviousStep(idx) && setCurrentStepIndex(idx)}
                                        >
                                            {(isPreviousStep(idx) || (isLastStep(idx) && isCurrentStep(idx))) ? (
                                                <FontAwesomeIcon icon={faCheckCircle} style={{ color: "#006435", fontSize: "1.5rem" }} />
                                            ) : (
                                                <span
                                                    style={{
                                                        display: "inline-block",
                                                        width: 24,
                                                        height: 24,
                                                        borderRadius: "50%",
                                                        background: isCurrentStep(idx) ? "#005eb8" : "#e5e5e5",
                                                        color: isCurrentStep(idx) ? "#fff" : "#333",
                                                        lineHeight: "24px",
                                                        fontWeight: "bold"
                                                    }}
                                                >
                                                    {idx + 1}
                                                </span>
                                            )}
                                            <div style={{ fontSize: 12, marginTop: 4 }}>{stepLabels[step]}</div>
                                        </div>
                                    ))}
                                </div>
                                <div style={{ marginTop: 16 }}>
                                    {stepContent[steps[currentStepIndex]]}
                                </div>
                            </>
                        ) : (
                            <Radios>
                                {steps.map((step, idx) => (
                                    <React.Fragment key={step}>
                                        {(isPreviousStep(idx) || (isLastStep(idx) && isCurrentStep(idx))) ? (
                                            <div
                                                className="completed-tick"
                                                aria-label={`${stepLabels[step]} completed`}
                                                role="radio"
                                                aria-checked="true"
                                                tabIndex={-1}
                                                style={{
                                                    display: "flex",
                                                    alignItems: "center",
                                                    cursor: isPreviousStep(idx) ? "pointer" : "default",
                                                    marginBottom: 8,
                                                    color: "black",
                                                }}
                                                onClick={() => isPreviousStep(idx) && setCurrentStepIndex(idx)}
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
                                                disabled={true}
                                                checked={isCurrentStep(idx)}
                                                onChange={() => { }}
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
                        )}
                    </div>
                </form>
            </Row>
        </Container>
    );
};

export default LeftProgress;