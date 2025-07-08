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

        <ListGroup variant="flush" className="text-start border-0">

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/' ? 'active' : ''}`}
                onClick={() => handleItemClick('/')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                <SecuredLink to="/home">Home</SecuredLink>
            </ListGroup.Item>

            <FeatureSwitch feature={FeatureDefinitions.ReportReidentify}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/report' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/report')}>
                    <FontAwesomeIcon icon={faLineChart} className="me-2 fa-icon" />
                    <SecuredLink to="/report">Report Re-identification</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.SinglePatientReidentify}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/reIdentification' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/reIdentification')}>
                    <FontAwesomeIcon icon={faPerson} className="me-2 fa-icon" />
                    <SecuredLink to="/reIdentification">Re-identify Single Patient</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.CsvReidentify}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/csvReIdentification' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/csvReIdentification')}>
                    <FontAwesomeIcon icon={faListAlt} className="me-2 fa-icon" />
                    <SecuredLink to="/csvReIdentification">Re-Identify Dataset</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.CsvWorklist}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/csvReIdentificationWorklist' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/csvReIdentificationWorklist')}>
                    <FontAwesomeIcon icon={faTable} className="me-2 fa-icon" />
                    <SecuredLink to="/csvReIdentificationWorklist">My Dataset Worklist</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Projects}>
                <SecuredComponent allowedRoles={securityPoints.impersonation.view}>
                    <ListGroup.Item
                        className={`bg-dark text-white ${activePath === '/project' ? 'active' : ''}`}
                        onClick={() => handleItemClick('/project')}>
                        <FontAwesomeIcon icon={faProjectDiagram} className="me-2 fa-icon" />
                        <SecuredLink to="/project">Projects</SecuredLink>
                    </ListGroup.Item>
                </SecuredComponent>
            </FeatureSwitch>

        </ListGroup>
    );
}

export default MenuComponent;