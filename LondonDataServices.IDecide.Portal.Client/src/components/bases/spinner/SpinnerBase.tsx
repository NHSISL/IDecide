import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleNotch } from '@fortawesome/free-solid-svg-icons'

export const SpinnerBase = () => {
    return (
        <div
            style={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                width: '50%',
                height: '100%',
                minHeight: '150px'
            }}
        >
        <FontAwesomeIcon
            icon={faCircleNotch}
            spin
            size="10x"
            className="loadingSpinner p-2"
                style={{ color: "#005eb8", fontSize:"50px" }}
            />
        </div>
    );
}