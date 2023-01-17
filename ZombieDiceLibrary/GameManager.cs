using ZombieDiceLibrary.Models;

namespace ZombieDiceLibrary
{
    public class GameManager : IDisposable
    {
        private static System.Timers.Timer? timer;

        private int maxGames;

        private int minutesBeforeClose;

        public event Action OnChange;

        private void Notify() => OnChange?.Invoke();

        
        public List<Game> Games { get; private set; }
        public GameManager(GameManagerConfiguration configuration)
        {
            Games = new();

            maxGames = configuration.MaxGames;

            minutesBeforeClose = configuration.MinutesBeforeClose;

            timer = new(10000);

            timer.Elapsed += (sender, eventArgs) => HandleTimer();

            timer.Start();
        }

        // Every 10 seconds check for and remove stale games.
        private void HandleTimer()
        {
            // Get a list of game ids to remove.

            var idsToRemove = new List<string>();

            foreach(var game in Games)
            {
                var now = DateTime.Now;

                var difference = now - game.LastModified;

                if (difference.Minutes > minutesBeforeClose || game.Players.Count == 0)
                {
                    idsToRemove.Add(game.Id);
                }
            }

            // Remove the games.

            foreach(var id in idsToRemove)
            {
                var game = Games.FirstOrDefault(game=> game.Id == id);

                if (game != null)
                {
                    game.CloseGame();

                    Games.Remove(game);
                }
            }

            Notify();
        }

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
            if (timer != null)
            {
                timer.Elapsed -= (sender, eventArgs) => HandleTimer();

                timer.Dispose();

                timer = null;
            }
        }
    }
}
