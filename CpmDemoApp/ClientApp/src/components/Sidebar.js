import React from 'react';
import './Sidebar.css';
import {agents as agentsMap} from '../config';

const Sidebar = ({ agents, onSelectAgent }) => {

  const getAgentName = (agentId) => {
    const agent = agentsMap.find(agent => agent.id === agentId);
    return agent ? agent.name : 'Unknown Agent';
  };
  return (
    <div className="sidebar">
      <h3>Agents</h3>
      <ul>
        {agents.map((agentId) => (
          <li key={agentId} onClick={() => onSelectAgent(agentId)}>
            {getAgentName(agentId)}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Sidebar;