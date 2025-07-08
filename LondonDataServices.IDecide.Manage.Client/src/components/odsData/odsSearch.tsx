import { FunctionComponent, useState } from "react";
import { ListGroup, ListGroupItem, Form, Spinner, InputGroup } from "react-bootstrap";
import { odsDataViewService } from "../../services/views/odsData/odsDataViewService";
import { OdsData } from "../../models/odsData/odsData";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";

type OdsSearchProps = {
    selectedRecords: OdsData[],
    setSelectedOrganisation: (odsRecord: OdsData | undefined) => void,
    selectedOrganisation: OdsData | undefined
};

const OdsSearch: FunctionComponent<OdsSearchProps> = (props) => {
    const { setSelectedOrganisation, selectedOrganisation } = props;
    
    const [searchTerm, setSearchTerm] = useState("");
    const { data, isLoading } = odsDataViewService.useGetAllOdsData(searchTerm)

   

    return (
        <>
            <Form>
                <Form.Group>
                    <InputGroup>
                        <Form.Control value={searchTerm} onChange={(e) => { setSearchTerm(e.target.value) }} placeholder="Search"></Form.Control>
                        <InputGroup.Text onClick={() => {
                            setSelectedOrganisation(undefined);
                            setSearchTerm("");
                        }}><FontAwesomeIcon icon={faTimes} /></InputGroup.Text>
                    </InputGroup>
                </Form.Group>
            </Form>
            <br />
            {searchTerm &&
                <>
                { isLoading && <Spinner/> }
                    <ListGroup>
                        {
                        data?.pages.map((i) => <>{i.data.map(x =>
                            <ListGroupItem active={selectedOrganisation?.id == x.id} onClick={() => setSelectedOrganisation(x)}>{x.organisationName} ({x.organisationCode})
                            </ListGroupItem>)}
                        </>)
                        }
                    </ListGroup>
                </>
            }
        </>
        
    );
};

export default OdsSearch;