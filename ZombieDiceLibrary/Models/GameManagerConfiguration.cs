namespace ZombieDiceLibrary.Models
{
    /// <summary>
    /// Represents a configuration for the game manager service.
    /// Instantiated in Program.cs from appsettings.json and passed to the GameManager constructor.
    /// </summary>
    public class GameManagerConfiguration
    {
        /// <summary>
        /// Represents the maximum allowed concurrent game instances.
        /// </summary>
        public int MaxGames { get; set; } = 1000;
        /// <summary>
        /// Represents the minutes of inactivity before a game is closed.
        /// </summary>
        public int MinutesBeforeClose { get; set; } = 15;
        /// <summary>
        /// Represents the seconds between checking of stale games.
        /// </summary>
        public int IntervalSeconds { get; set; } = 60;
    }
}
