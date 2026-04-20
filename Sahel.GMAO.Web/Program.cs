using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Sahel.GMAO.Infrastructure.Data;
using Sahel.GMAO.Infrastructure.Services;
using Sahel.GMAO.Core.Interfaces;
using Sahel.GMAO.Web.Auth;
using Sahel.GMAO.Web.Components;
using Sahel.GMAO.Web.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddSignalR();

// Database
builder.Services.AddDbContextFactory<GmaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication("GmaoCookie")
    .AddCookie("GmaoCookie", options =>
    {
        options.LoginPath = "/login";
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(sp => (CustomAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipementService, EquipementService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IDtService, DtService>();
builder.Services.AddScoped<INotificationService, Sahel.GMAO.Web.Services.NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GmaoHub>("/gmaohub");

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GmaoDbContext>>();
    using var context = await contextFactory.CreateDbContextAsync();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context);
}

app.Run();
