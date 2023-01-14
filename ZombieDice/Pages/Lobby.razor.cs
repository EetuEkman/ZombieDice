using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ZombieDiceLibrary;
using ZombieDiceLibrary.Models;

namespace ZombieDice.Pages
{
    public partial class Lobby : IDisposable
    {
        [Inject]
        public ProtectedSessionStorage Storage { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        [Inject]
        public UserManager userManager { get; set; }

        [Inject]
        public GameManager gameManager { get; set; }

        private string? userId;

        private User? user;

        private string? errorText;

        private string? password;

        public Lobby() {}

        private async void OnChangeHandler()
        {
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            userManager.OnChange += OnChangeHandler;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var result = await Storage.GetAsync<string>("userId");

            if (result.Success == false)
            {
                navigationManager.NavigateTo("/", true);

                return;
            }

            userId = result.Value;

            user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                navigationManager.NavigateTo("/", true);

                return;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        void IDisposable.Dispose()
        {
            if (userId != null)
            {
                userManager.RemoveUser(userId);
            }

            userManager.OnChange -= OnChangeHandler;
        }
        void CreateGame()
        {
            if (user is null)
            {
                return;
            }

            var gameId = gameManager.NewGame(user, password);

            navigationManager.NavigateTo("/game/" + gameId);
        }

        void JoinGame(string id, string? password)
        {
            var game = gameManager.Games.FirstOrDefault(g => g.Id == id);

            if (game is null)
            {
                errorText = "Game not found.";

                return;
            }

            if (password != game.Password && game.Password is not null)
            {
                errorText = "Wrong password.";

                return;
            }

            navigationManager.NavigateTo("/game/" + game.Id);
        }
    }
}
