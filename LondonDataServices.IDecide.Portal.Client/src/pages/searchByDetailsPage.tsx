import SearchByDetails from "../components/searchByDetails/searchByDetails";

export const SearchByDetailsPage = ({
    onBack,
    nextStep,
}: {
    onBack: () => void;
    nextStep: () => void;
}) => (
    <SearchByDetails onBack={onBack} nextStep={nextStep} />
);

export default SearchByDetailsPage;