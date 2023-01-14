using ZombieDiceLibrary.Models;

namespace ZombieDiceLibrary
{
    public class GameManager : IDisposable
    {
        private int maxGames;

        public event Action OnChange;

        private void Notify() => OnChange?.Invoke();

        public GameManager(GameManagerConfiguration configuration)
        {
            Games = new();

            maxGames = configuration.MaxGames;
        }
        public List<Game> Games { get; private set; }

        /// <summary>
        /// Attemps to create a new game instance.
        /// Pushes the game instance to the game manager's list of games.
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="password"></param>
        /// <returns>Game id on success, null otherwise.</returns>
        public string? NewGame(User creator, string? password = "")
        {
            if (Games.Count > maxGames)
            {
                return null;
            }

            var id = Utilities.GetRandomString(5);

            var game = new Game(id, creator, password);

            Games.Add(game);

            Notify();

            return game.Id;
        }

        public string? JoinGame(string id, string? password, User user)
        {
            var index = Games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return null;
            }

            var gamePassword = Games[index].Password;

            if (String.IsNullOrEmpty(gamePassword) == false)
            {
                if (String.Equals(password, gamePassword) == false)
                {
                    return null;
                }
            }

            var player = new Player()
            {
                Id = user.Id,
                Name = user.Name,
                Brains = 0
            };

            Games[index].PlayerJoins(player);

            return Games[index].Id;
        }

        public void RemoveGame(Game game)
        {
            Games.Remove(game);

            Notify();
        }

        void IDisposable.Dispose()
        {
            
        }
    }
}

        /*
        public GameManager(string id, User user)
        {
            Players = new List<User>()
            {
                user
            };
        }
        public List<User> Players { get; private set; }
        public bool GameHasStarted { get; set; } = false;
        public int TurnCount { get; private set; } = 0;
        public int WhoseTurnIndex { get; private set; } = 0;

        public void Turn()
        {

        }

        public void NextTurn()
        {
            WhoseTurnIndex++;

            if (WhoseTurnIndex > Players.Count - 1)
            {
                WhoseTurnIndex = 0;
            }
        }

        private Die GetZombieDie(int Id, Colors color)
        {
            switch (color)
            {
                case Colors.Green:
                    return new Die(Id, Colors.Green);
                case Colors.Yellow:
                    return new Die(Id, Colors.Yellow);
                case Colors.Red:
                    return new Die(Id, Colors.Red);
                default:
                    throw new NotImplementedException();
            }
        }

        public List<Die> AllDice()
        {
            var dice = new List<Die>()
            {
                GetZombieDie(1, Colors.Green),
                GetZombieDie(2, Colors.Green),
                GetZombieDie(3, Colors.Green),
                GetZombieDie(4, Colors.Green),
                GetZombieDie(5, Colors.Green),
                GetZombieDie(6, Colors.Green),
                GetZombieDie(7, Colors.Yellow),
                GetZombieDie(8, Colors.Yellow),
                GetZombieDie(9, Colors.Yellow),
                GetZombieDie(10, Colors.Yellow),
                GetZombieDie(11, Colors.Red),
                GetZombieDie(12, Colors.Red),
                GetZombieDie(13, Colors.Red),
            };

            return dice;
        }

        private Random random = Random.Shared;

        /// <summary>
        /// Represents the cup with all the unrolled dice
        /// </summary>
        public List<Die> DiceCup { get; set; }

        /// <summary>
        /// Represents the dice the player picked up from the cup
        /// </summary>
        public List<Die> PickedDice { get; set; }

        /// <summary>
        /// Represents the saved dice already rolled this turn
        /// </summary>
        public List<Die> SavedDice { get; set; }

        public List<Die> DiceResult { get; set; }

        public int Brains { get; set; }


        /// <summary>
        /// Pick up dice from the cup
        /// </summary>
        /// <param name="amount">How many dice are picked up</param>
        public void PickFromDiceCup(int amount)
        {
            if (PickedDice.Count >= 3)
            {
                return;
            }

            // Randomly choose the dice to pick up from the dice cup

            var indices = new int[amount];

            for (int i = 0; i < amount; i++)
            {
                var index = random.Next(0, DiceCup.Count - 1);

                // The same die can only be picked up once

                while (indices.Contains(index))
                {
                    index = random.Next(0, DiceCup.Count - 1);
                }

                indices[i] = index;
            }

            // Pick up the dice

            foreach (var index in indices)
            {
                this.PickedDice.Add(DiceCup[index]);
            }

            // Remove the picked up dice from the dice cup

            var ids = new List<int>();

            foreach (var index in indices)
            {
                var dice = DiceCup[index];

                ids.Add(dice.Id);
            }

            DiceCup.RemoveAll(die => ids.Contains(die.Id));
        }

        /// <summary>
        /// Roll the picked up dice
        /// </summary>
        public void Roll()
        {
            var rolledDice = new List<Die>();

            PickedDice.ForEach(dice => rolledDice.Add(dice.Roll()));

            DiceResult = rolledDice;

            PickedDice.Clear();
        }

        private void CheckResults()
        {
            if (DiceResult == null)
            {
                return;
            }

            var brains = 0;
            var footPrints = 0;
            var shotguns = 0;

            DiceResult.ForEach(dice =>
            {
                if (dice.Facing == null)
                {
                    Console.WriteLine("ZombieDiceManager.cs: CheckResults: dice.facing == null.");

                    return;
                }

                switch (dice.Facing.Value)
                {
                    case ZombieDieFacings.Brain:
                        brains++;
                        break;
                    case ZombieDieFacings.Footprints:
                        footPrints++;
                        break;
                    case ZombieDieFacings.Shotgun:
                        shotguns++;
                        break;
                }
            }
            );

            if (brains > 3)
            {
                Console.WriteLine("Lose, lose all saved brains.");
            }

            // Prompt

            // Offer reroll

            // 
        }
    }
}
        */

/*
    private HubConnection? hubConnection;

    private string? userName;
    private string? userNameError;

    private string? message;
    private List<string> messages = new List<string>();

    public bool isConnected => hubConnection?.State == HubConnectionState.Connected;

    private string? userId;

    private async void joinUser()
        {
        if (String.IsNullOrWhiteSpace(userName))
            {
            userNameError = "User name can not be empty.";

        return;
        }

        if (userName.Length < 2)
            {
            userNameError = "User name minimum length is 2 characters.";
        }

        if (userName?.Length > 32)
            {
            userNameError = "User name maximum length is 32 characters.";

        return;
        }

        await JoinUser();
    }

    private async void clickSend()
        {
    await Send();
    }

    private void createGame()
        {
        if(hubConnection is not null)
            {
            var userId = hubConnection.ConnectionId;

            navigationManager.NavigateTo($"/game/{userId}");
    }
    }

    protected override async Task OnInitializedAsync()
        {
        hubConnection = new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/zombiedicehub"))
        .WithAutomaticReconnect()
        .Build();

        hubConnection.On<User>("NewUser", async user =>
            {
            var userIndex = userManager.Users.FindIndex(u => u.Id == user.Id);

            userId = userManager.Users[userIndex].Id;

            await InvokeAsync(StateHasChanged);
        });

        hubConnection.On<string, string, string>("ReceiveMessage", (id, user, message) =>
            {
            var line = $"{id} {user}: {message}";

            messages.Add(line);

            InvokeAsync(StateHasChanged);
        });

        hubConnection.On<string>("SystemMessage", message =>
            {
            messages.Add(message);

            InvokeAsync(StateHasChanged);
        });

        hubConnection.On("StateChanged", () =>
            {
            InvokeAsync(StateHasChanged);
        });

        userManager.Notify += () => InvokeAsync(StateHasChanged);

        await hubConnection.StartAsync();
    }

    private async Task JoinUser()
        {
        if (hubConnection is not null)
            {
            await hubConnection.SendAsync("JoinUser", userName);
    }
    }

    private async Task Send()
        {
        if (hubConnection is not null)
            {
            await hubConnection.SendAsync("SendMessage", userName, message);
            }
    }



    public async ValueTask DisposeAsync()
        {
        if (hubConnection is not null)
            {
            await hubConnection.DisposeAsync();
    }
        }

    */