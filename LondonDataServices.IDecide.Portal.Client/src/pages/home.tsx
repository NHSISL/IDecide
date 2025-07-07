import React from "react";
import { Container, Row, Col } from "react-bootstrap";
import { Button, Card, Checkboxes, Footer, Radios, ArrowRight } from "nhsuk-react-components";

export const Home = () => {
    return (
        <Container className="mt-4">
            <Row>
                <Card.Group>
                    <Card.GroupItem width="one-half" className="">

                        <Card.Content>
                            <Card.Heading className="nhsuk-heading-m">
                                <ArrowRight /> The OptOut Process
                            </Card.Heading>
                            <Card.Description>
                                <form
                                    style={{
                                        padding: 20
                                    }}
                                >
                                    <ea>

                                        <Radios
                                            hint="Please follow the path."
                                            id="example-conditional"
                                            name="example"
                                        >
                                            <Radios.Radio
                                                conditional={<Checkboxes id="impairments" name="impairments"></Checkboxes>}
                                                id="hello1"
                                                value="yes"
                                            >
                                                Confirm your details
                                            </Radios.Radio>
                                            <Radios.Radio
                                                id="hello2"
                                                value="no"
                                            >
                                                Positive Confirmation that we have the correct details on record.
                                            </Radios.Radio>
                                        </Radios>
                                    </ea>
                                </form>
                            </Card.Description>
                        </Card.Content>

                    </Card.GroupItem>
                    <Card.GroupItem width="one-half">

                        <Card.Content>
                            <Card.Heading className="nhsuk-heading-m">
                                Confirm your NHS Number
                            </Card.Heading>
                            <Card.Description>
                                
                            </Card.Description>
                        </Card.Content>
                    </Card.GroupItem>
                </Card.Group>
            </Row>

        </Container>
    );
};