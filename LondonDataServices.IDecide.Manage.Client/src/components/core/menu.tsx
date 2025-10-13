import { faHome, faUser } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation, useNavigate } from 'react-router-dom'; 

const MenuComponent: React.FC = () => {
    const location = useLocation();
    const navigate = useNavigate(); 

    const handleItemClick = (path: string) => {
        navigate(path); 
    };

    return (
        <ListGroup variant="flush" className="text-start border-0">
            <ListGroup.Item
                className={`bg-dark text-white ${location.pathname === '/home' ? 'active' : ''}`}
                onClick={() => handleItemClick('/home')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                Home
            </ListGroup.Item>
            <ListGroup.Item
                className={`bg-dark text-white ${location.pathname === '/nhsNumberSearch' ? 'active' : ''}`}
                onClick={() => handleItemClick('/nhsNumberSearch')}>
                <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                Search Patient PDS
            </ListGroup.Item>
            <ListGroup.Item className="bg-dark text-white fw-bold mt-4" disabled>
                Audit
            </ListGroup.Item>
            <ListGroup.Item
                className={`bg-dark text-white ${location.pathname === '/patientSearch' ? 'active' : ''}`}
                onClick={() => handleItemClick('/patientSearch')}>
                <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                Audit Search
            </ListGroup.Item>

            <ListGroup.Item className="bg-dark text-white fw-bold mt-4" disabled>
                Configuration
            </ListGroup.Item>
            <ListGroup.Item
                className={`bg-dark text-white ${location.pathname === '/consumers' ? 'active' : ''}`}
                onClick={() => handleItemClick('/consumers')}>
                <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                Consumers
            </ListGroup.Item>
        </ListGroup>
    );
};

export default MenuComponent;
