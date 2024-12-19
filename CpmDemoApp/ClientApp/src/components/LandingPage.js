import React, { useState } from 'react';
import './LandingPage.css';
import {agents} from '../config';

const LandingPage = ({ onGeneratePortals }) => {
  const [selectedAgents, setSelectedAgents] = useState([]);

  const handleCheckboxChange = (agentId) => {
    setSelectedAgents((prevSelectedAgents) =>
      prevSelectedAgents.includes(agentId)
        ? prevSelectedAgents.filter((id) => id !== agentId)
        : [...prevSelectedAgents, agentId]
    );
  };

  const handleGeneratePortals = () => {
    console.log('in side landing page for agents:', agents); // Add logging
    onGeneratePortals(selectedAgents);
  };

  return (
    <div className="landing-page">
      <h2>Select Agents</h2>
      <ul>
        {agents.map((agent) => (
          <li key={agent.id}>
            <label>
              <input
                type="checkbox"
                checked={selectedAgents.includes(agent.id)}
                onChange={() => handleCheckboxChange(agent.id)}
              />
              {agent.name}
            </label>
          </li>
        ))}
      </ul>
      <button onClick={handleGeneratePortals}>Continue</button>
    </div>
  );
};

export default LandingPage;