import React, { useState } from 'react';
import './ChatWindow.css';
import { channelId, agents } from '../config';

const ChatWindow = ({ messagesServiceClient, selectedConversation, selectedAgent, messages, setMessages }) => {
    const [inputMessage, setInputMessage] = useState('');
    const [isWhisper, setIsWhisper] = useState(false); // New state for the checkbox
    const [summaryPopupVisible, setSummaryPopupVisible] = useState(false);
    const [summary, setSummary] = useState('');

    const parseSystemMessage = (message) => {
        const parser = new DOMParser();
        const xml = parser.parseFromString(message, "text/xml");
        const tagName = xml.documentElement.tagName.toLowerCase();

        const getAgentName = (id) => {
            const agent = agents.find((agent) => id.includes(agent.id));
            return agent ? agent.name : id.split("_")[1]; // Fallback to extracting ID if no match is found
        };

        switch (tagName) {
            case "deletemember": {
                const targets = Array.from(xml.getElementsByTagName("target")).map((t) =>
                    getAgentName(t.textContent)
                );
                return `${targets.join(", ")} removed from the conversation.`;
            }
            case "addmember": {
                const targets = Array.from(xml.getElementsByTagName("target")).map((t) =>
                    getAgentName(t.textContent)
                );
                return `${targets.join(", ")} added to the conversation.`;
            }
            default:
                return "Unknown system message.";
        }
    };


    const getSenderName = (msgItem) => {
        const agent = agents.find(agent => msgItem?.senderCommunicationIdentifier?.includes(agent.id));
        return agent ? agent.name : "Customer";
    };

    const handleSendMessage = async () => {
        if (selectedConversation) {
            let sendMessageResponse = await messagesServiceClient.pathUnchecked(`/messages/conversations/${selectedConversation.id}/messages:sendMessage`).post({
                contentType: "application/json",
                body: {
                    request: {
                        kind: "text",
                        content: (isWhisper ? "[Internal] " : "") + inputMessage,
                        to: ["+11234567890"], // Dummy value
                        channelRegistrationId: channelId
                    },
                    outboundDeliveryStrategy: isWhisper ? "EmployeesOnly" : "allChannels" // Update strategy
                }
            });

            let messagesResult = await messagesServiceClient.pathUnchecked(`/messages/conversations/${selectedConversation.id}/messages`).get();
            let messagesValue = messagesResult.body.value;
            setMessages(messagesValue.sort((a, b) => a.sequenceId - b.sequenceId));
            setInputMessage('');
        }
    };

    const handleSummarizeConversation = async () => {
        if (selectedConversation) {
            let summarizeResponse = await messagesServiceClient.pathUnchecked(`/messages/conversations/${selectedConversation.id}/messages:summarize`).get();
            let summary = summarizeResponse.body.summary;

            console.log("Summarize Response:", summarizeResponse);

            setSummary(summary);
            setSummaryPopupVisible(true);
        }
    };

    return (
        <div className="chat-window">
            <div className="header">
                <h4>Chat</h4>
                {selectedConversation && <button className="summarize-button" onClick={handleSummarizeConversation}>Summarize</button>}
            </div>
            <div className="messages">
                {messages?.length > 0 ?
                    messages.map((msgItem) => {
                        if (!msgItem.message || !msgItem.message.content || msgItem.message.content.trim().length === 0) {
                            return null;
                        }
                        const isSystemMessage = msgItem.message.content.startsWith('<');
                        const messageContent = isSystemMessage
                            ? parseSystemMessage(msgItem.message.content)
                            : msgItem.message.content;

                        return (
                            <div
                                key={msgItem.id}
                                className={`message ${isSystemMessage
                                    ? 'system-message'
                                    : msgItem?.senderCommunicationIdentifier?.includes(selectedAgent)
                                        ? 'sender'
                                        : 'receiver'
                                    }`}
                            >
                                {!isSystemMessage && <div className="sender-name">{getSenderName(msgItem)}</div>}
                                {messageContent}
                            </div>
                        );
                    })
                    : <p>Select a conversation</p>
                }
            </div>
            {selectedConversation && <div className="input-container">
                <input
                    type="text"
                    value={inputMessage}
                    onChange={(e) => setInputMessage(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                    placeholder="Type your message here..."
                />
                <div className="whisper-checkbox">
                    <input
                        type="checkbox"
                        id="whisper"
                        checked={isWhisper}
                        onChange={(e) => setIsWhisper(e.target.checked)}
                    />
                    <label htmlFor="whisper">Internal</label>
                </div>
                <button onClick={handleSendMessage}>Send</button>
            </div>}
            {summaryPopupVisible && <div className="popup-container">
                <span className="close-button" onClick={() => setSummaryPopupVisible(false)}>&times;</span>
                <div className="popup-content">
                    <h3>Conversation Summary</h3>
                    <p>{summary}</p>
                </div>
            </div>
            }
        </div>
    );
};

export default ChatWindow;
