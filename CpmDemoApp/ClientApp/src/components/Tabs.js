import React, { useState } from 'react';
import './Tabs.css';

const Tabs = ({ children }) => {
  const [activeTab, setActiveTab] = useState(children?.props?.label);

  const handleTabClick = (label) => {
    setActiveTab(label);
  };

  return (
    <div className="tabs">
      <div className="tab-list">
        {children.map((child) => {
          if (!child.props) return null;
          const { label } = child.props;
          return (
            <div
              key={label}
              className={`tab ${label === activeTab ? 'active' : ''}`}
              onClick={() => handleTabClick(label)}
            >
              {label}
            </div>
          );
        })}
      </div>
      <div className="tab-content">
        {children.map((child) => {
          if (!child.props) return null;
          if (child.props.label === activeTab) {
            return <div key={child.props.label}>{child.props.children}</div>;
          }
          return null;
        })}
      </div>
    </div>
  );
};

export default Tabs;