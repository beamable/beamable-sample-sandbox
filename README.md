# beamable-sample-sandbox
Public repository for sharing Beamable sample code for various purposes and in various states of completeness or usability.
# Ping-Pong Turn-Based Game System with Beamable

This system implements a simple turn-based "ping-pong" game where two players take turns to ping each other. The system uses Beamable’s **microservices** and **notification** features to handle turn management and real-time communication between players.

The Unity client subscribes to notifications to listen for turn updates, sends pings to the opponent, and checks whose turn it is to update the UI. The server maintains the turn state in a shared `TurnData` document and provides endpoints for the client to ping the opponent and check the current turn.

## Components Breakdown

### 1. TurnSystem (Client)

The `TurnSystem` class handles the following responsibilities:
- Initializes the player's context and subscribes to ping notifications.
- Sends a ping to the opponent using the server's microservices.
- Listens for notifications to know when it is the player's turn.
- Updates the UI to display whether it's the player's turn or the opponent's turn.

#### Key functionality in the client:
- **`SendPingButton`**: When the player presses a button, this function sends a ping to the opponent using the server's microservices.
- **`HandlePingNotification`**: When a notification is received indicating the opponent's turn, this function processes the notification and updates the player's UI accordingly.
- **`UpdateTurnStatus`**: Queries the server to check if it is the player's turn and updates the turn indicator UI.

### 2. Service (Server)

The `Service` microservice is responsible for:
- Managing the `TurnData` (which stores the current turn and the two players involved).
- Sending notifications to players when their turn comes up.
- Providing endpoints for the client to ping the opponent and check the current turn.

#### Key server functionality:
- **`Ping`**: Switches the turn to the opponent, updates the `TurnData`, and sends a notification to the opponent.
- **`IsPlayerTurn`**: Checks if it is the player's turn by comparing the player's ID with the current turn in `TurnData`.
- **`GetFromPlayer`**: Fetches the `FromPlayer` field from the `TurnData`, allowing the client to set the opponent player on initialization.

### Data Storage in Microstorage

The turn state is stored in Beamable's **Microstorage** to persist game data between player turns.

## Beamable Features Used

### Notifications
The system uses Beamable's **NotificationService** to send real-time updates (`PingNotification`) when the turn switches from one player to another.

### Microservices
The server-side logic for turn management is encapsulated in Beamable **microservices**, which provide callable methods (`Ping`, `IsPlayerTurn`, `GetFromPlayer`) for the client to interact with.

### Microstorage
The `TurnData` is stored using Beamable's **microstorage** to maintain persistent state across player turns.
