# ZombieDice

Zombie dice dice game ported to Blazor web app.

The app is hosted in [Azure] (https://zombiedice20230227014601.azurewebsites.net/)

## Motivation

I wanted to learn Microsoft Blazor. I find the idea of running C# code interesting.

I've had a lot of fun playing a dice game called Zombie dice by Steve Jackson with my friends. I thought that making recreating that game as a Blazor web app would be a great way to get some familiarity with the Blazor.

The game logic itself was a challenge, but I decided to try to add multiplayer which brought about complexity. I would have to find a way to have a shared game instance and events and handle players joining and leaving without making the game freeze.

Implementing an AI players would make the single player much more interesting. 

Also I would have to find a way to close ununused instances to prevent stale games just piling up taking up resources.

The idea is that there is a sort of a "lobby" where you can choose to create a game or join an existing game. I thought about adding a chat feature in the lobby but decided against it for the time being at least.

When the user joins or creates a game, they're redirected to the "room" page.

The suggested player count is between 2 to 8 players so I decided to limit the number of players to 8. I don't allow spectators at this point.

## ZombieDiceLibrary

I have learned that a class library is a popular to separate "business logic" from the "display layer", so that's what I did here.

The ZombieDiceLibrary contains the models and different classes. I added a reference to the class library and it seems to work great.

### Game Manager

I imagined that the game instances would be managed by the Game Manager service which I added to the services as a singleton. The game manager would have a list of the game instances, and would spin up new game instances and close down the old ones.

When a user navigates to a game room page, the game manager would be injected into it. Then, game instance can be fetched from the game manager's list of games by the game id.

### Game instance

The game instance holds the game's state and methods. When a room page gets the reference to the game instance it subscribes to the game's events which is fired on state change. This way the users pages are kept up to date. I suspect that this happens via SignalR.
