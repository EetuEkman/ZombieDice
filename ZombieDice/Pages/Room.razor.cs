using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ZombieDiceLibrary;
using System.Text.Json;
using ZombieDiceLibrary.Models;

namespace ZombieDice.Pages
{
    public partial class Room : IDisposable
    {
        [Parameter]
        public string? Id { get; set; }

        [Inject]
        public GameManager GameManager { get; set; }
        // Holds the game state.
        public Game Game { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ProtectedSessionStorage Storage { get; set; }

        public Player Player { get; private set; }

        public void SetPlayer(Player player)
        {
            this.Player = player;

            OnChangeHandler();
        }

        private async void OnChangeHandler()
        {
            await InvokeAsync(StateHasChanged);
        }

        private async void OnCloseHandler()
        {
            NavigationManager.NavigateTo("/", true);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private async Task OnFirstRender()
        {
            // The game state is stored in the game manager.

            var game = GameManager.Games.FirstOrDefault(game => game.Id == Id);

            if (game is null)
            {
                NavigationManager.NavigateTo("/", true);

                return;
            }

            Game = game;

            Game.OnChange += OnChangeHandler;

            Game.OnClose += OnCloseHandler;

            // Get the user from session storage.

            var result = await Storage.GetAsync<string>("user");

            if (result.Success == false)
            {
                NavigationManager.NavigateTo("/", true);

                return;
            }

            var json = result.Value;

            var user = JsonSerializer.Deserialize<User>(json);

            var player = new Player()
            {
                Id = user.Id,
                Name = user.Name,
                Brains = 0
            };

            SetPlayer(player);

            // Add user to the list of players.

            Game.PlayerJoins(player);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await OnFirstRender();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        void IDisposable.Dispose()
        {
            // Delete user from the session storage.
            // Prevents coming back to the game with browser forward button without checks.

            Storage.DeleteAsync("user");

            if (Player is not null)
            {
                Game.PlayerLeaves(Player);
            }

            if (Game is not null)
            {
                Game.OnChange -= OnChangeHandler;

                Game.OnClose -= OnCloseHandler;
            }
        }
    }
}
