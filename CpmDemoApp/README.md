# README: CpmDemoApp

## Introduction
The CpmDemoApp is a demo application designed to showcase the threaded conversation functionalities and is consisted of two core components:

1. **Main ASP.NET Web App**
   - Built using ASP.NET.
   - Includes controllers, models, views, and various helper classes.
   - Hosts the **Admin Portal experience** and orchestrates the entire flow.
   - Utilizes the .NET SDK to facilitate the Admin Portal functionalities.

2. **Client App**
   - Located at `CpmDemoApp\ClientApp`.
   - Implements the **Agent Portal experience** in JavaScript.
   - Utilizes the JavaScript SDK to facilitate the Agent Portal functionalities.

## Prerequisites
To get started, ensure the following requirements are met:

- An Azure account with an active subscription. For details, see [Create an account for free](https://aka.ms/Mech-Azureaccount).
- [.NET SDK 7.0 or later](https://dotnet.microsoft.com/download).
- An Azure Communication Services resource. For details, see [Create and manage Communication Services resources](https://learn.microsoft.com/azure/communication-services/quickstarts/create-communication-resource).
- A WhatsApp Channel under Azure Communication Services. For details, see [Register WhatsApp business account](https://learn.microsoft.com/azure/communication-services/quickstarts/advanced-messaging/whatsapp/connect-whatsapp-business-account).
- Enable Message Analysis for the WhatsApp Channel. For details, see [Enable Message Analysis with Azure OpenAI](https://learn.microsoft.com/azure/communication-services/quickstarts/advanced-messaging/message-analysis/message-analysis-with-azure-openai-quickstart).
- Generate 5 MRIs for the ACS resource to represent agent identities. See [Create an identity](https://learn.microsoft.com/azure/communication-services/quickstarts/identity/access-tokens?tabs=windows&pivots=platform-azcli#create-an-identity).

## Configuration Setup

### Main ASP.NET Web App
1. Rename `appsettings.sample.json` to `appsettings.json` in the root folder.
2. Update the following configurations in `appsettings.json`:
   - Connection string
   - Channel registration ID

   *(Both can be found in the Azure Portal under your ACS resource.)*
3. Open `CpmDemoApp\Util\Data.cs` and update the `AgentLists` property with the agent MRIs.

### Client App
1. Rename `config.sample.js` to `config.js` in `CpmDemoApp\ClientApp\src`.
2. Update the following configurations in `config.js`:
   - `endpointUrl`, `channelId`, and `connectionString`
   *(Again these values are available in the Azure Portal under your ACS resource.)*
   - Add the agent MRI list in the same order as defined in `Data.cs`.


## SDKs Setup
At this point, the SDKs are not publicly available. Please contact Gloria Li (gelli) to obtain the local binaries. Once you have them, follow the instructions below to integrate them:

### JavaScript SDK
1. Place the package folder under CpmDemoApp\ClientApp.
   - The folder structure should look like CpmDemoApp\ClientApp\package, containing the JavaScript binaries

### .NET SDK:
1. Set Up a Local NuGet Source:
    - Go to **Tools > Options** in Visual Studio.
    - Navigate to **NuGet Package Manager > Package Sources**.
    - Click the **"+"** button to add a new source.
        - **Name:** Provide a name for your local source (e.g., `LocalPackages`).
        - **Source:** Browse to the folder containing your `.nupkg` file.

2. Install the Package:
    - Right-click on your project in Solution Explorer.
    - Select **"Manage NuGet Packages…"**.
    - Switch to the **"Browse"** tab.
    - Use the drop-down menu at the top right to select your local source (e.g., `LocalPackages`).
    - Search for your package and click **"Install"**.

3. Verify Installation:
    - Check the **"Dependencies"** node in Solution Explorer to confirm that the package is listed.


## Event Grid Setup
To receive notification messages and message analysis:

1. **Set up Event Grid Subscription**:
   - Subscribe to `advancedmessagereceived` and `advancedmessageanalysiscompleted` events.
   - Choose **Web Hook** as the endpoint type.
   - Use the webhook endpoint URL:
     - For deployed web apps: `https://yourapp.azurewebsites.net/webhook`.
     - For local testing: Use an ngrok URL (e.g., `https://your-ngrok-url/webhook`).
     - For further details on how to subscribe to Event Grid, see [Subscribe to Azure Communication Services events](https://learn.microsoft.com/azure/communication-services/quickstarts/events/subscribe-to-events?pivots=platform-azp) 

2. **Local Testing with ngrok**:
   - Download and configure ngrok to expose your local host.
   - Note down the ngrok-generated public URL.

3. **Deploy the Web App**:
   - Follow the [Quickstart: Publish an ASP.NET web app](https://learn.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-aspnet-web-app?view=vs-2022&tabs=azure).

## Running the App

1. **Build the Client Application**:
   - Navigate to `CpmDemoApp\ClientApp` and run:
     ```
     npm install
     npm run build
     ```
     *(The build process copies the client app artifacts to the `wwwroot` directory of the ASP.NET Web App, ensuring seamless integration.)*

2. **Start the Application**:
   - Run the `CpmDemoApp` solution to launch the application.

## Additional Notes
- Ensure all configurations are accurately updated before running the application.
- For further assistance, reach out to the Gloria Li (gelli).
