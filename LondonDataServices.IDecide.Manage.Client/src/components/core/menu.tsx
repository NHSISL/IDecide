import { faHome, faCog, faUser, faUserDoctor, faAddressBook, faIdBadge } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';
import { SecuredLink } from '../securitys/securedLinks';
import securityPoints from '../../securityMatrix';
import { SecuredComponent } from '../securitys/securedComponents';
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
                <SecuredLink to="/home">Home</SecuredLink>
            </ListGroup.Item>

            <FeatureSwitch feature={FeatureDefinitions.UserAccess}>
                <SecuredComponent allowedRoles={securityPoints.userAccess.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/userAccess' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/userAccess')}>
                        <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                        <SecuredLink to="/userAccess">User Access</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <SecuredComponent allowedRoles={securityPoints.configuration.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/accessAudit' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/accessAudit')}>
                        <FontAwesomeIcon icon={faFontAwesomeLogoFull} className="me-2 fa-icon" />
                        <SecuredLink to="/accessAudit">Access Audit</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>

            {/*<ListGroup.Item*/}
            {/*    className={`bg-dark text-white ${activePath === '/impersonationContext' ? 'active' : ''}`}*/}
            {/*    onClick={() => handleItemClick('/impersonationContext')}>*/}
            {/*    <FontAwesomeIcon icon={faUserFriends} className="me-2 fa-icon" />*/}
            {/*    <SecuredLink to="/impersonationContext">Impersonation Context</SecuredLink>*/}
            {/*</ListGroup.Item>*/}

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <SecuredComponent allowedRoles={securityPoints.configuration.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/csvIdentificationRequest' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/csvIdentificationRequest')}>
                        <FontAwesomeIcon icon={faIdBadge} className="me-2 fa-icon" />
                        <SecuredLink to="/csvIdentificationRequest">Csv Identification</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <SecuredComponent allowedRoles={securityPoints.configuration.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/pdsData' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/pdsData')}>
                        <FontAwesomeIcon icon={faAddressBook} className="me-2 fa-icon" />
                        <SecuredLink to="/pdsData">PDS Data</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <SecuredComponent allowedRoles={securityPoints.configuration.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/odsData' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/odsData')}>
                        <FontAwesomeIcon icon={faUserDoctor} className="me-2 fa-icon" />
                        <SecuredLink to="/odsData">Ods Data</SecuredLink>
                    </ListGroup.Item>
                    </SecuredComponent>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <SecuredComponent allowedRoles={securityPoints.configuration.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/configuration/lookups' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/configuration/lookups')}>
                        <FontAwesomeIcon icon={faCog} className="me-2 fa-icon" />
                        <SecuredLink to="/configuration/lookups">Config - Lookup Reasons</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>
        </ListGroup>
    );
}

export default MenuComponent;