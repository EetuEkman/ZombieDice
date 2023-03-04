using Microsoft.AspNetCore.ResponseCompression;
using ZombieDiceLibrary;
using ZombieDiceLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" }
        );
});

// Handles user related functionality.

builder.Services.AddSingleton<UserManager>();

// Maximum allowed concurrent game instances.

var maxGames = builder.Configuration.GetValue<int>("GameManager:MaxGames");

// Minutes of inactivity before game is closed.

var minutesBeforeClose = builder.Configuration.GetValue<int>("GameManager:MinutesBeforeClose");

// Seconds between every stale game check

var intervalSeconds = builder.Configuration.GetValue<int>("GameManager:IntervalSeconds");

var gameManagerConfiguration = new GameManagerConfiguration()
{
    MaxGames = maxGames,
    MinutesBeforeClose = minutesBeforeClose,
    IntervalSeconds = intervalSeconds
};

// Keeps a track of, handles creation and deletion of game instances.

builder.Services.AddSingleton<GameManager>(sp => new GameManager(gameManagerConfiguration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
