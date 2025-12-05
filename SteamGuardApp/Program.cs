using SteamGuardApp;
using SteamGuardApp.Components;
using SteamGuardApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// User Settings
builder.Services.AddCascadingValue(sp => new Settings());
// Steam Guard Timer
builder.Services.AddTransient(sp => new SteamGuardTimerService(TimeSpan.FromSeconds(1)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
