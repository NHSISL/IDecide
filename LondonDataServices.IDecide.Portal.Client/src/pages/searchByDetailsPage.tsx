import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const SearchByDetailsPage = ({
    onBack,
    powerOfAttourney = false,
}: {
    onBack: () => void;
    powerOfAttourney?: boolean;
}) => (
    <SearchByDetails
        onBack={onBack}
        powerOfAttourney={powerOfAttourney}
    />
);

export default SearchByDetailsPage;