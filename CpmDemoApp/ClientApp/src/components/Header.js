import React from 'react';
import './Header.css';

const Header = ({ agentName }) => {
  return (
      <div className="header">
          <div className="left">
              <img src="~/assets/logo.png" alt="Shop Logo" class="logo" />
              Blossom Flower Studio
          </div>
          <div className="center">
            Agent portal
          </div>
          <div className="right">
            <span className="account-symbol">👤</span>
            <span className="account-name">{agentName}</span>
          </div>
      </div>
  );
};

export default Header;