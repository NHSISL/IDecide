import React from "react";
import SearchByNhsNumber from "../components/SearchNhsNumber/searchByNhsNumber";
import { Container } from "react-bootstrap";

interface SearchByNhsNumberPageProps {
    onIDontKnow: () => void;
    powerOfAttorney?: boolean;
}

export const SearchByNhsNumberPage: React.FC<SearchByNhsNumberPageProps> = ({ onIDontKnow, powerOfAttorney }) => (
    <Container>
        <SearchByNhsNumber onIDontKnow={onIDontKnow} powerOfAttorney={powerOfAttorney} />
    </Container>
);

export default SearchByNhsNumberPage;