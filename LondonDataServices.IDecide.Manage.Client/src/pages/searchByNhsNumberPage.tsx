import SearchByNhsNumber from "../components/SearchNhsNumber/searchByNhsNumber";
import { Card, Container } from "nhsuk-react-components";

export const SearchByNhsNumberPage = () => (
    <Container fluid>
        <Card cardType="feature">
            <Card.Content>
                <Card.Heading>NHS Number Search</Card.Heading>
                <Card.Description>
                    <SearchByNhsNumber />
                </Card.Description>
            </Card.Content>
        </Card>
    </Container>
);

export default SearchByNhsNumberPage;