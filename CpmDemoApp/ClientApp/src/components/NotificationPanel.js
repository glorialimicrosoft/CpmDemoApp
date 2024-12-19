import React, { useEffect, useState } from 'react';
import './NotificationPanel.css';

const NotificationPanel = ({ notifications, selectedAgent }) => {
  const [localNotifications, setLocalNotifications] = useState([]);

  useEffect(() => {
    if (selectedAgent && notifications) {
      console.log("notificationMap in USE EFFECT", notifications);
      setLocalNotifications(notifications);
    }
  }, [notifications, selectedAgent]);

  return (
    <div className="notification-panel">
      <h2>Latest Notifications!</h2>
      {localNotifications.length > 0 ? 
          localNotifications.map((notification) => (
            <div className="notification-item">
              <strong>{notification.type}</strong>{notification.content} 
            </div>
          ))
              : <p>No notifications yet.</p>
          }
    </div>
  );
};

export default NotificationPanel;