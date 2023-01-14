using ZombieDiceLibrary.Models;

namespace ZombieDiceLibrary
{
    public class Game
    {
        public Game(string id, User creator, string? password)
        {
            Id = id;

            var player = new Player()
            {
                Id = creator.Id,
                Name = creator.Name,
                Brains = 0
            };

            Players.Add(player);

            Password = password;

            DiceCup = AllDice();
        }

        public string Id { get; private set; }

        /// <summary>
        /// Represents the players participating in the current game.
        /// </summary>
        public List<Player> Players { get; private set; } = new();

        public void PlayerJoins(Player player)
        {
            if (Players.FirstOrDefault(p => p.Id == player.Id) is null)
            {
                Players.Add(player);
            }

            SendMessage($"{player.Name} joined.");
        }

        public void PlayerLeaves(Player player)
        {
            if (Players.FirstOrDefault(p => p.Id == player.Id) is not null)
            {
                Players.Remove(player);

                SendMessage($"{player.Name} left.");
            }
        }

        /// <summary>
        /// Event to be fired on state change that the components can listen to.
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Fires of the OnChange event so that the components using the game state know the state has changed.
        /// </summary>
        private void Notify() => OnChange?.Invoke();
        /// <summary>
        /// Represents which player's turn it is. Index of a player's list.
        /// </summary>
        public int WhoseTurn { get; private set; }

        /// <summary>
        /// Represents the current turn phase.
        /// </summary>
        public TurnPhase? TurnPhase { get; private set; }

        public void SetTurnPhase(TurnPhase turnPhase)
        {
            // Check the last turn.

            if (turnPhase == ZombieDiceLibrary.TurnPhase.Pick)
            {
                if (WhoseTurn == WhoWentOverThreshold)
                {
                    var winner = Players.MaxBy(p => p.Brains);

                    HasStarted = false;

                    SendMessage($"{winner.Name} has won the game!.");

                    return;
                }
            }

            TurnPhase = turnPhase;

            Notify();
        }

        /// <summary>
        /// Ending the turn with 13 or more brains causes one more turn before the game ends.
        /// Holds the index of the player who ended their turn with 13 or more brains.
        /// </summary>
        public int? WhoWentOverThreshold { get; set; }

        public void NextTurn()
        {
            if (HasStarted == false)
            {
                return;
            }

            var newWhoseTurn = WhoseTurn + 1;

            if (newWhoseTurn > Players.Count - 1)
            {
                newWhoseTurn = 0;
            }

            if (newWhoseTurn == WhoWentOverThreshold)
            {
                var winner = Players.MaxBy(p => p.Brains);

                SendMessage($"{winner.Name} has won the game!.");

                End();

                return;
            }

            if (Players[newWhoseTurn].Brains >= 13)
            {
                WhoWentOverThreshold = newWhoseTurn;

                SendMessage($"{Players[newWhoseTurn].Name} has 13 or more brains. Last round.");
            }
            
            SetTurnPhase(ZombieDiceLibrary.TurnPhase.Pick);

            Brains = 0;

            ResetDice();

            WhoseTurn = newWhoseTurn;

            SendMessage($"{Players[WhoseTurn].Name}'s turn.");
        }
        /// <summary>
        /// Represents the dice in the dice cup.
        /// At the start of turn, all dice are in the cup.
        /// </summary>
        public List<Die> DiceCup { get; private set; } = new();
        public void SetDiceCup(List<Die> dice)
        {
            DiceCup = dice;
        }
        /// <summary>
        /// Represents the dice in hand ready to be rolled.
        /// </summary>
        public List<Die> DiceInHand { get; private set; } = new();

        public void SetDiceInHand(List<Die> dice)
        {
            DiceInHand = dice;

            Notify();
        }
        /// <summary>
        /// Represents the dice that have been rolled.
        /// </summary>
        public List<Die> RolledDice { get; private set; } = new();
        public void SetRolledDice(List<Die> dice)
        {
            RolledDice = dice;

            Notify();
        }

        /// <summary>
        /// Represents the dice set aside, brain and shotgun results when the player decided to keep rolling.
        /// </summary>
        public List<Die> SavedDice { get; private set; } = new();
        public void SetSavedDice(List<Die> dice)
        {
            SavedDice= dice;

            Notify();
        }
        /// <summary>
        /// Represents the password to join the game.
        /// </summary>
        public string? Password { get; private set; }

        /// <summary>
        /// Used to keep tally of the brains in the case of dice running out of the cup.
        /// Brains are returned to the cup. Shotguns stay.
        /// </summary>
        public int Brains { get; set; } = 0;
        /// <summary>
        /// Used to check if the game has been started.
        /// </summary>
        public bool HasStarted { get; private set; } = false;

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Start()
        {
            HasStarted = true;

            WhoseTurn = 0;

            Brains = 0;

            Players.ForEach(player => player.Brains = 0);

            SetTurnPhase(ZombieDiceLibrary.TurnPhase.Pick);

            SendMessage("Game has started.");
        }

        /// <summary>
        /// End the game.
        /// </summary>
        public void End()
        {
            HasStarted = false;

            Brains = 0;

            WhoseTurn = 0;

            WhoWentOverThreshold = 0;

            SendMessage("Game has ended.");
        }

        /// <summary>
        /// Represents the message log.
        /// </summary>
        public List<string> Messages { get; private set; } = new();

        public void SendMessage(string message)
        {
            var messages = new List<string>(Messages);

            var time = DateTime.Now.ToLocalTime().ToShortTimeString();

            messages.Add(time + " " + message);

            if (messages.Count >= 20)
            {
                messages.RemoveAt(0);
            }

            Messages = messages;

            Notify();
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

        // Returns all the 13 dice in unrolled state.
        private List<Die> AllDice()
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

        // Returns all the dice to the dice cup.
        public void ResetDice()
        {
            DiceCup = AllDice();

            DiceInHand.Clear();

            RolledDice.Clear();

            SavedDice.Clear();

            Notify();
        }

        public void SaveDice()
        {
            if (TurnPhase != ZombieDiceLibrary.TurnPhase.Decide)
            {
                return;
            }

            // Get all the dice with the result "footprints".

            var toBeRerolled = RolledDice.FindAll(die => die.Facing!.Value == ZombieDieFacings.Footprints);

            // Pick up the dice to be rerolled.

            DiceInHand = toBeRerolled;

            // Find all the dice that are to be saved, "brains" and "shotguns".

            var toBeSaved = RolledDice.FindAll(die => die.Facing!.Value != ZombieDieFacings.Footprints);

            // Combine existing saved dice with the dice to be saved.

            var savedDice = SavedDice.Concat(toBeSaved).ToList();

            // Save the dice.

            SavedDice = savedDice;

            // Rolled dice are cleared.

            RolledDice = new List<Die>();

            Notify();
        }

        public void RollDice()
        {
            var rolledDice = new List<Die>();

            // All the dice in hand are rolled.

            DiceInHand.ForEach(dice => rolledDice.Add(dice.Roll()));

            // Dice have been rolled, the hand is empty.

            DiceInHand = new List<Die>();

            // Resolve the results, count shotguns, move to the next game phase.

            ResolveRoll(rolledDice);
        }

        private void ResolveRoll(List<Die> rolledDice)
        {
            // Count the shotguns in both the rolled dice and saved dice, round ends on 3 or more shotguns.

            var shotguns = CountShotguns(rolledDice) + CountShotguns(SavedDice);

            if (shotguns >= 3)
            {
                // End turn.

                RolledDice = rolledDice;

                Brains = 0;

                SetTurnPhase(ZombieDiceLibrary.TurnPhase.End);

                var name = Players[WhoseTurn].Name;

                SendMessage($"{name} rolled {shotguns} shotguns. {name}'s turn has ended.");

                return;
            }

            RolledDice = rolledDice;

            SetTurnPhase(ZombieDiceLibrary.TurnPhase.Decide);

            SendMessage($"{Players[WhoseTurn].Name} rolled the dice.");
        }

        public void KeepRolling()
        {
            SaveDice();

            // Clear the rolled dice.

            RolledDice = new List<Die>();

            if (DiceInHand.Count == 3)
            {
                // Skip the pick phase.

                SetTurnPhase(ZombieDiceLibrary.TurnPhase.Roll);
            }
            else
            {
                SetTurnPhase(ZombieDiceLibrary.TurnPhase.Pick);
            }

            SendMessage($"{Players[WhoseTurn].Name} decides to keep rolling.");
        }

        // Count the brains and update the tally.
        public void Stay()
        {
            var brains = CountBrains(RolledDice) + CountBrains(SavedDice) + Brains;

            Players[WhoseTurn].Brains += brains;

            SendMessage($"{Players[WhoseTurn].Name} stays, saving {brains} brains.");

            NextTurn();
        }

        private void BrainsToCup()
        {
            var brains = SavedDice.Where(die => die.Facing?.Value == ZombieDieFacings.Brain).ToList();

            Brains = brains.Count();

            var diceCup = DiceCup.Concat(brains).ToList();

            DiceCup = diceCup;

            var shotguns = SavedDice.Where(die => die.Facing?.Value == ZombieDieFacings.Shotgun).ToList();

            SavedDice = shotguns;

            Notify();
        }

        public void PickDice()
        {
            // There should never be more than 3 dice in hand.

            if (DiceInHand.Count > 3)
            {
                throw new Exception($"DiceInHand.Count > 3, Count was {DiceInHand.Count}");
            }

            // If there are 3 dice in hand, skip to the roll phase.
            // Happens if all 3 dice are footsteps.

            if (DiceInHand.Count == 3)
            {
                SetTurnPhase(ZombieDiceLibrary.TurnPhase.Roll);

                return;
            }

            // Three dice are picked at a time. Footsteps are rerolled.

            // E.g. The dice result was 1 brain and 2 footsteps.
            // Player decices to keep rolling.
            // 2 yellow footstep dice are taken to hand.
            // 1 die is needed.
            // The die is taken from the remaining dice in the dice cup randomly.

            // E.g. there are no dice in hand, pick 3 dice, 3 - 0 = 3 dice picked.

            // E.g. 1 die in hand, 3 - 1 = 2 dice picked.

            var pickCount = 3 - DiceInHand.Count;

            var diceCup = new List<Die>(DiceCup);

            var diceInHand = new List<Die>(DiceInHand);

            // There might not be enough dice in the cup.

            // Keep shotguns in the saved dice. Take all the brains and put them back into the dice cup.

            if (pickCount > diceCup.Count)
            {
                var savedDice = new List<Die>(SavedDice);

                // Get the saved brains to put into the dice cup.

                var savedBrains = savedDice.Where(die => die.Facing?.Value == ZombieDieFacings.Brain).ToList();

                // Take a note of the excess brains.

                Brains += savedBrains.Count();

                // Add the brains back to the dice cup.

                diceCup = diceCup.Concat(savedBrains).ToList();

                diceCup.ForEach(die => die.Facing = null);

                // Get the saved shotguns to put into the saved dice.

                var savedShotguns = savedDice.Where(die => die.Facing?.Value == ZombieDieFacings.Shotgun).ToList();

                savedDice = savedShotguns;

                SavedDice = savedDice;
            }

            // Pick the wanted amount of dice.

            for (var i = pickCount; i > 0; i--)
            {
                // Take a die from the dice cup at random.

                var index = Random.Shared.Next(0, diceCup.Count);

                /*
                while (DiceCup.ElementAtOrDefault(i) == null)
                {
                    index = Random.Shared.Next(0, DiceCup.Count);
                }
                */

                var die = diceCup[index];

                diceInHand.Add(die);

                diceCup.RemoveAt(index);
            }

            DiceCup = diceCup;

            DiceInHand = diceInHand;

            if (pickCount == 1)
            {
                SetTurnPhase(ZombieDiceLibrary.TurnPhase.Roll);

                SendMessage($"{Players[WhoseTurn].Name} picks one die.");

                return;
            }

            SetTurnPhase(ZombieDiceLibrary.TurnPhase.Roll);

            SendMessage($"{Players[WhoseTurn].Name} picks {pickCount} dice.");
        }

        private int CountShotguns(ICollection<Die> dice)
        {
            int count = 0;

            foreach(var die in dice)
            {
                if (die.Facing?.Value == ZombieDieFacings.Shotgun)
                {
                    count++;
                }
            }

            return count;
        }

        private int CountBrains(ICollection<Die> dice)
        {
            int count = 0;

            foreach (var die in dice)
            {
                if (die.Facing?.Value == ZombieDieFacings.Brain)
                {
                    count++;
                }
            }

            return count;
        }
    }
}