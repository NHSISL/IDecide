// LondonDataServices.IDecide.Portal.Client\src\pages\searchByNhsNumberPage.tsx
import React from "react";
import SearchByNhsNumber from "../components/SearchNhsNumber/searchByNhsNumber";
import { Container } from "react-bootstrap";

export const SearchByNhsNumberPage = ({ onIDontKnow }: { onIDontKnow: () => void }) => (
    <Container>
        <SearchByNhsNumber onIDontKnow={onIDontKnow} />
    </Container>
);

export default SearchByNhsNumberPage;