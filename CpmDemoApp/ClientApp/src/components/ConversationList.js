import React from 'react';
import './ConversationList.css';

const ConversationList = ({ conversations, onSelectConversation }) => {
  return (
    <div className="conversation-list">
      <h2>My Conversations</h2>
      <ul>
        {conversations.map((conversation) => (
          <li key={conversation.id} className="conversation-item">
            {conversation.id}
            <button onClick={() => onSelectConversation(conversation)}>Get Messages</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ConversationList;