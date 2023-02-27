# ZombieDice

Zombie dice dice game ported to Blazor web app.

The app is hosted in [Azure](https://zombiedice20230227014601.azurewebsites.net/)

## Motivation

I wanted to learn Microsoft Blazor. Blazor is a web framework that enables developers to create web apps using C#. I find the idea of running C# code in the browser interesting so I decided to try it out.

I've had a lot of fun playing a dice game called Zombie dice by Steve Jackson with my friends. I thought that recreating that game as a Blazor web app would be a great way to get some familiarity with the technology.

The game logic itself was a challenge, but I decided to try to add multiplayer which brought about complexity. I would have to find a way to have a shared game state, with shared events and handle players joining and leaving without making the game freeze.

Implementing an AI players would make the single player much more interesting.

Also I would have to find a way to close ununused instances to prevent stale games just piling up taking up resources. I also thought to limit the maximum concurrent game instances to say 1000 games.

The idea is that there is a sort of a "lobby" where you can choose to create a game or join an existing game. I thought about adding a chat feature in the lobby but decided against it for the time being at least.

First, user is directed to a front page. The front page is sort of a "lobby" where user must choose a player name, choose to either create game or join existing game.

When the user joins or creates a game, they're redirected to the "room" page.

The suggested player count is between 2 to 8 players so I decided to limit the number of players to 8. I don't allow spectators at this point.

## ZombieDiceLibrary

I've learned that a class library is a popular option to separate "business logic" from the "display layer", so that's what I did here.

The ZombieDiceLibrary contains the models and different classes. I added a reference to the class library and it works great.

### Game Manager

I imagined that the game instances would be managed by the Game Manager service which I added to the services as a singleton. The game manager holds a reference to games in a list of the game instances. Manager spins up new game instances and closes down old ones.

When a user navigates to a game room page, the game manager service is made avaialble using dependency injection. Then, game instance can be fetched from the game manager's list of games by the game id.

### Game instance

The game instance holds the game state and methods. When a room page gets the reference to the game instance it subscribes to the game's events which are fired on state change. This way the users pages are kept up to date.