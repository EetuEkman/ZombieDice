using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;
using ZombieDice.Data;
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

builder.Services.AddSingleton<UserManager>();

var maxGames = builder.Configuration.GetValue<int>("GameManager:MaxGames");

var gameManagerConfiguration = new GameManagerConfiguration() { MaxGames = maxGames };

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
