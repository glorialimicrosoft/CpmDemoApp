const AzureCommunicationTokenCredential = require("@azure/communication-common").AzureCommunicationTokenCredential;
const CreateMessagesServiceClient = require("@azure-rest/communication-messages").default;
const ConversationMessagesClient = require("@azure-rest/communication-messages").ConversationMessagesClient;

// The ACS endpoint

let endpointUrl = '<replace with your resource endpoint>';

// The user access token
let userAccessToken ="<replace with access token>";

let cred = new AzureCommunicationTokenCredential('<USER_ACCESS_TOKEN>');

let messagesServiceClient = CreateMessagesServiceClient(endpointUrl, cred);

let conversationMessagesClient = new ConversationMessagesClient(endpointUrl, userAccessToken);

// List Conversations
let conversationsResult = await messagesServiceClient.path("/messages/conversations").get();// List Messages in a conversation

let messages = await messagesServiceClient.path("/messages/conversations/{conversationId}/messages", "<replace with your conversation id>").get();

// Send Message to a conversation

let sendMessageResponse = await messagesServiceClient.path("/messages/conversations/{conversationId}/messages:sendMessage", '<replace with your conversation id>').post({    contentType: "application/json",    body: {      request:       {        kind: "text",        content: "hello world!",        to: ["+1234567890"],  channelRegistrationId: "00000000-0000-0000-0000-00000000"    },      outboundDeliveryStrategy: "allChannels"    }  }); 
    
// open notifications channel

await conversationMessagesClient.startRealtimeNotifications();

// subscribe to new notification
conversationMessagesClient.on("chatMessageReceived", (e) => {  console.log("Notification chatMessageReceived!");  });