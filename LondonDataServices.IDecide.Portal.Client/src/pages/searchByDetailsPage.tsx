import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const SearchByDetailsPage = ({
    onBack,
    powerOfAttorney = false,
}: {
    onBack: () => void;
        powerOfAttorney?: boolean;
}) => (
    <SearchByDetails
        onBack={onBack}
        powerOfAttorney={powerOfAttorney}
    />
);

export default SearchByDetailsPage;