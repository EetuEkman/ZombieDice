using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ZombieDiceLibrary;
using System.Text.Json;
using ZombieDiceLibrary.Models;

namespace ZombieDice.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        public UserManager UserManager { get; set; }
        [Inject]
        public GameManager GameManager { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ProtectedSessionStorage Storage { get; set; }

        private async void OnChangeHandler()
        {
            await InvokeAsync(StateHasChanged);
        }

        private string name = "";
        private string nameError = "";
        private string password = "";
        private string createError = "";
        private string gameName = "";
        private string joinError = "";
        private string joinPassword = "";

        protected override void OnInitialized()
        {
            GameManager.OnChange += OnChangeHandler;

            base.OnInitialized();
        }

        private bool validateName(string input)
        {
            nameError = "";

            if (String.IsNullOrWhiteSpace(input) == true)
            {
                nameError = "Name is required.";

                return false;
            }

            if (input.Length < 2 || input.Length > 32)
            {
                nameError = "Name must be between 2 and 32 characters.";

                return false;
            }

            return true;
        }

        private async Task persistUser(User user)
        {
            var json = JsonSerializer.Serialize(user);

            await Storage.SetAsync("user", json);
        }

        private async void createGame()
        {
            createError = "";

            if (validateName(this.name) == false)
            {
                return;
            }

            var user = UserManager.NewUser(this.name);
            await persistUser(user);

            var gameId = GameManager.NewGame(user, password);

            if (gameId is null)
            {
                createError = "Error creating the game.";

                return;
            }

            NavigationManager.NavigateTo("/room/" + gameId);
        }

        private async void joinGame()
        {
            joinError = "";

            if (validateName(this.name) == false)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(gameName))
            {
                joinError = "Game name is required.";

                return;
            }

            var game = GameManager.Games.FirstOrDefault(game => game.Id == gameName);

            if (game is null)
            {
                joinError = "Game not found.";

                return;
            }

            if (String.IsNullOrWhiteSpace(game.Password) == false)
            {
                if (game.Password.Equals(password) == false)
                {
                    joinError = "Wrong password.";

                    return;
                }
            }

            if (game.Players.Count >= 8)
            {
                joinError = "Game is full.";

                return;
            }

            if (game.HasStarted)
            {
                joinError = "Game in progress.";

                return;
            }

            var user = UserManager.NewUser(this.name);

            await persistUser(user);

            NavigationManager.NavigateTo("/room/" + game.Id);
        }

        void IDisposable.Dispose()
        {
            GameManager.OnChange -= OnChangeHandler;
        }
    }
}
