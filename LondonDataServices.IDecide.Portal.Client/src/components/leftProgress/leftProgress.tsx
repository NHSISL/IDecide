import { Fieldset, Radios } from "nhsuk-react-components";
import React from "react";
import { Container, Row } from "react-bootstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from "react-i18next";

const steps = ["nhs", "details", "confirm", "choice", "notify", "thanks"];

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
    const { t: translate } = useTranslation();

    const isMobile = useIsMobile();

    const isCurrentStep = (idx: number) => idx === currentStepIndex;
    const isPreviousStep = (idx: number) => idx < currentStepIndex;
    const isLastStep = (idx: number) => idx === steps.length - 1;

    // Get localized labels and content
    const stepLabels: Record<string, string> = steps.reduce((acc, step) => {
        acc[step] = translate(`LeftProgress.steps.${step}`);
        return acc;
    }, {} as Record<string, string>);

    const stepContent: Record<string, React.ReactNode> = steps.reduce((acc, step) => {
        acc[step] = <p>{translate(`LeftProgress.content.${step}`)}</p>;
        return acc;
    }, {} as Record<string, React.ReactNode>);

    return (
        <Container>
            <Row>
                <form style={{ padding: 20 }}>
                    <div className="leftProgressPadding">
                        <Fieldset.Legend>
                            {/*<h2>{translate("LeftProgress.legend")}</h2>*/}
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