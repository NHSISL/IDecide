import SearchByNhsNumber from "../components/SearchNhsNumber/searchByNhsNumber";
import { Card, Container } from "nhsuk-react-components";

interface SearchByNhsNumberPageProps {
    onIDontKnow: () => void;
    powerOfAttourney?: boolean;
}

export const SearchByNhsNumberPage: React.FC<SearchByNhsNumberPageProps> = ({ onIDontKnow, powerOfAttourney }) => (
    <Container fluid>
        <Card cardType="feature">
            <Card.Content>
                <Card.Heading>NHS Number Search</Card.Heading>
                <Card.Description>
                    <SearchByNhsNumber onIDontKnow={onIDontKnow} powerOfAttourney={powerOfAttourney} />
                </Card.Description>
            </Card.Content>
        </Card>
    </Container>
);

export default SearchByNhsNumberPage;