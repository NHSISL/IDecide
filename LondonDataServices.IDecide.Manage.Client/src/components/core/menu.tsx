import { faHome,faUser } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';;
import { FeatureDefinitions } from '../../featureDefinitions';
import { FeatureSwitch } from '../accessControls/featureSwitch';
import { faFontAwesomeLogoFull } from '@fortawesome/free-solid-svg-icons/faFontAwesomeLogoFull';

const MenuComponent: React.FC = () => {
    const location = useLocation();
    const [activePath, setActivePath] = useState(location.pathname);

    const handleItemClick = (path: string) => {
        setActivePath(path);
    };

    return (

        <ListGroup variant="flush" className="text-start border-0">
            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/' ? 'active' : ''}`}
                onClick={() => handleItemClick('/')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                Home
            </ListGroup.Item>

            <FeatureSwitch feature={FeatureDefinitions.UserAccess}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/userAccess' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/userAccess')}>
                        <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                       User Access
                    </ListGroup.Item>
            </FeatureSwitch>
        </ListGroup>
    );
}

export default MenuComponent;