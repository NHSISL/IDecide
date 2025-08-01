import React from "react";
import SearchByNhsNumber from "../components/SearchNhsNumber/searchByNhsNumber";
import { Container } from "react-bootstrap";

interface SearchByNhsNumberPageProps {
    onIDontKnow: () => void;
    powerOfAttourney?: boolean;
}

export const SearchByNhsNumberPage: React.FC<SearchByNhsNumberPageProps> = ({ onIDontKnow, powerOfAttourney }) => (
    <Container>
        <SearchByNhsNumber onIDontKnow={onIDontKnow} powerOfAttourney={powerOfAttourney} />
    </Container>
);

export default SearchByNhsNumberPage;