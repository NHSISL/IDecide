import { faHome, faPerson, faProjectDiagram } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';
import { SecuredLink } from '../securitys/securedLinks';
import { faListAlt } from '@fortawesome/free-solid-svg-icons/faListAlt';
import { faTable } from '@fortawesome/free-solid-svg-icons/faTable';
import { FeatureSwitch } from '../accessControls/featureSwitch';
import { FeatureDefinitions } from '../../featureDefinitions';
import { faLineChart } from '@fortawesome/free-solid-svg-icons/faLineChart';
import securityPoints from '../../securityMatrix';
import { SecuredComponent } from '../securitys/securedComponents';


const MenuComponent: React.FC = () => {
    const location = useLocation();
    const [activePath, setActivePath] = useState(location.pathname);

    const handleItemClick = (path: string) => {
        setActivePath(path);
    };

    return (

        
    );
}

export default MenuComponent;