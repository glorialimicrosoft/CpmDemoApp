import React, { useState, useEffect, useRef } from 'react';
import './App.css';
import ChatWindow from './components/ChatWindow';
import ConversationList from './components/ConversationList';
import NotificationPanel from './components/NotificationPanel';
import { endpointUrl, connectionString } from './config';
import { CommunicationIdentityClient } from '@azure/communication-identity';

const CreateMessagesServiceClient = require("@azure-rest/communication-messages").default;
const AzureCommunicationTokenCredential = require("@azure/communication-common").AzureCommunicationTokenCredential;
const ConversationMessagesClient = require("@azure-rest/communication-messages").ConversationMessagesClient;


function App() {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(null);
    const [notifications, setNotifications] = useState([]);
    const [messagesServiceClient, setMessagesServiceClient] = useState(null);
    const [conversationMessagesClient, setConversationMessagesClient] = useState(null);
    const [conversations, setConversations] = useState([]);
    const [selectedConversation, setSelectedConversation] = useState(null);
    const [messages, setMessages] = useState([]);
    const selectedConversationRef = useRef(null);

    useEffect(() => {
        selectedConversationRef.current = selectedConversation;
    }, [selectedConversation]);


    const getTokenForUser = async (userId) => {
        try {
            const identityClient = new CommunicationIdentityClient(connectionString);
            const user = { communicationUserId: userId };
            const tokenResponse = await identityClient.getToken(user, ['chat']);
            return tokenResponse.token;
        }
        catch (error) {
            console.error('Failed to get token:', error);
        }
    };

    const reorderMessages = (messagesValue) => {
        return messagesValue.sort((a, b) => a.sequenceId - b.sequenceId);
    };


    useEffect(() => {
        if (selectedConversation) {
            const fetchMessages = async () => {
                console.log('Fetching messages for conversation:', selectedConversation);
                console.log('Fetching messages for conversation:', selectedConversation?.id);
                let messagesResult = await messagesServiceClient.pathUnchecked(`/messages/conversations/${selectedConversation.id}/messages`).get();
                let messagesValue = messagesResult.body.value;
                console.log('Messages:', messagesValue);
                setMessages(reorderMessages(messagesValue));
            };
            fetchMessages();
        } else {
            setMessages([]);
        }
    }, [selectedConversation]);

    useEffect(() => {
        async function fetchUser() {
            try {
                const response = await fetch('/account/currentUser');

                if (response.ok) {
                    const userData = await response.json();
                    console.log(userData);
                    setUser(userData);
                } else {
                    console.error('Failed to fetch user data');
                }
            } catch (error) {
                console.error('Error fetching user data:', error);
            }
        }

        fetchUser();
    }, []);

    useEffect(() => {
        if (user !== null) {
            handleGeneratePortals();
        }
    }, [user]); // Trigger handleGeneratePortals when the user state changes


  useEffect(() => {
    const fetchData = async () => {
        if (user === null || token === null || messagesServiceClient === null) {
            console.log('messagesServiceClient is not set');
            return;
        }

        try {
            const conversationsResult = await messagesServiceClient.path("/messages/conversations").get();
            const conversations = conversationsResult.body.value;
            setConversations(conversations);
            setNotifications(null);
        }
        catch (error) {
            console.error('Failed to fetch conversations:', error);
        }
    };
    fetchData();
  }, [messagesServiceClient]);

    useEffect(() => {
        if (user === null || token === null || conversationMessagesClient === null) {
            console.log('conversationMessagesClient is not set');
            return;
        }

    const appendNotification = (notification) => {
        console.log("notification in appendNotification", notifications);
        setNotifications((prevNotifications) => {
            const safePrevNotifications = Array.isArray(prevNotifications) ? prevNotifications : [];
            return [...safePrevNotifications, notification];
        });
        console.log("notifications", notifications);
    };

    const startRealTimeNotifications = async () => {
     
      if (!conversationMessagesClient) {
        console.log('conversationMessagesClient is not set');
        return;
      }
      conversationMessagesClient.startRealtimeNotifications();
      conversationMessagesClient.on('realTimeNotificationConnected', handleRealTimeNotificationConnected);
      conversationMessagesClient.on('chatThreadCreated', handleChatThreadCreated);
      conversationMessagesClient.on('chatMessageReceived', handleChatMessageReceived);
    };

    const handleRealTimeNotificationConnected = (notification) => {
      
      appendNotification({
        Type: 'Connected',
        content: `You are now connected to real time notifications`,
        timestamp: notification.createdOn,
      });
    };

    const handleChatThreadCreated = (notification) => {
      appendNotification({
        Type: 'Conversation Created',
        content: `You were added to a new conversation with id ${notification.id}`,
        timestamp: notification.createdOn,
      });
    };

    const handleChatMessageReceived = (notification) => {
        if (notification.sender.communicationUserId === user.id) {
            return; // Do not append own message
        }

        appendNotification({
            Type: 'Message Received',
            content: `Message received in conversation ${notification.threadId}`,
            timestamp: notification.createdOn,
        });


        // Use the ref to access the latest selectedConversation
        if (selectedConversationRef.current?.id !== notification.threadId) {
            console.log("Message does not belong to the selected conversation, ignoring.");
            return;
        }

        // add the received message to the message list if the notification belongs to the current selected conversation
        let newMessage = {
            messageId: notification.id,
            message: {
                content: notification.message,
            },
            senderDisplayName: notification.senderDisplayName,
            senderCommunicationIdentifier: `https://noam-canary.msgapi.teams.microsoft.com:444/v1/users/ME/contacts/${notification.sender.communicationUserId}`,
            createdOn: new Date(notification.createdOn).toISOString(),
        };

        setMessages((prevMessages) => {
            const safePrevMessages = Array.isArray(prevMessages) ? prevMessages : [];
            return [...safePrevMessages, newMessage];
        });
    };


    startRealTimeNotifications();

    return () => {
      conversationMessagesClient.off('realTimeNotificationConnected', handleRealTimeNotificationConnected);
      conversationMessagesClient.off('chatThreadCreated', handleChatThreadCreated);
      conversationMessagesClient.off('chatMessageReceived', handleChatMessageReceived);
    };
  }, [conversationMessagesClient]);


    const handleSelectConversation = async (conversation) => {
        setSelectedConversation(conversation);
      };
  
    const handleGeneratePortals = async () => {
        const token = await getTokenForUser(user.id);
        const tokenCredential = new AzureCommunicationTokenCredential(token);
        const client = CreateMessagesServiceClient(endpointUrl, tokenCredential);
        const realTimeNotificationClient = new ConversationMessagesClient(endpointUrl, tokenCredential);
        realTimeNotificationClient.startRealtimeNotifications();

        setConversationMessagesClient(realTimeNotificationClient);
        setMessagesServiceClient(client);
        setToken(token);
    };

  return (
    <div className="App">
          {user === null ? (
              <div className="unauthorizedPage">
                  <h1>Please Log In First!</h1>
              </div>
      ) : (
        <div className="content">
          <div className="main-content">
            <div className="panel">
              <ConversationList conversations={conversations} onSelectConversation={handleSelectConversation} />
            </div>
            <div className="panel">
                <ChatWindow messagesServiceClient={messagesServiceClient} selectedConversation={selectedConversation} selectedAgent={user?.id} messages={messages} setMessages={setMessages} />
            </div>
            <div className="panel">
              <NotificationPanel notifications={notifications} selectedAgent = {user?.id} />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
