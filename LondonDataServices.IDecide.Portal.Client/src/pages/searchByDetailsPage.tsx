import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const SearchByDetailsPage = ({
    onBack,
    nextStep,
    powerOfAttourney = false,
}: {
    onBack: () => void;
    nextStep: () => void;
    powerOfAttourney?: boolean;
}) => (
    <SearchByDetails
        onBack={onBack}
        nextStep={nextStep}
        powerOfAttourney={powerOfAttourney}
    />
);

export default SearchByDetailsPage;