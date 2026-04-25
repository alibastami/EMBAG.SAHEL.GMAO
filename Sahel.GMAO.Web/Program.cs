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

Console.WriteLine(">>> SAHEL GMAO: Initializing process...");
try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = AppContext.BaseDirectory
    });

    builder.Configuration.SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

// Configure Serilog (Synced with DRH)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddSignalR();
builder.Services.AddAntiforgery();

// Database
builder.Services.AddDbContextFactory<GmaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication (Synced with DRH)
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(sp => (CustomAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddAuthorization();

// Application Services
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipementService, EquipementService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IWorkingProfileService, WorkingProfileService>();
builder.Services.AddScoped<IDtService, DtService>();
builder.Services.AddScoped<IFabricationService, FabricationService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<INotificationService, Sahel.GMAO.Web.Services.NotificationService>();
builder.Services.AddScoped<INatureTravailService, NatureTravailService>();
builder.Services.AddScoped<IConsignationService, ConsignationService>();
builder.Services.AddScoped<IBackupService, BackupService>();

var app = builder.Build();
Log.Information(">>> GMAO: Environment: {Env}", app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GmaoHub>("/gmaohub");

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        Log.Information(">>> GMAO: Starting database initialization...");
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GmaoDbContext>>();
        using var context = await contextFactory.CreateDbContextAsync();
        
        Log.Information(">>> GMAO: Checking/Applying migrations...");
        await context.Database.MigrateAsync();
        
        Log.Information(">>> GMAO: Seeding default data...");
        await DbInitializer.SeedAsync(context);
        
        Log.Information(">>> GMAO: Database is ready.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, ">>> GMAO: Fatal error during database initialization.");
        throw;
    }
}

    // --- Auto-Launch Browser Logic ---
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            var server = app.Services.GetService<Microsoft.AspNetCore.Hosting.Server.IServer>();
            var addressFeature = server?.Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
            var url = addressFeature?.Addresses.FirstOrDefault(a => a.StartsWith("http:")) ?? "http://localhost:6001";
            
            // Clean up address like http://[::]:6001 or http://+ to http://localhost:6001
            url = url.Replace("[::]", "localhost").Replace("0.0.0.0", "localhost").Replace("127.0.0.1", "localhost").Replace("+", "localhost");

            Log.Information(">>> GMAO: Opening browser at {Url}", url);
            
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to launch browser automatically.");
        }
    });

    // --- Launch Logic (Synced with DRH) ---
    try 
    {
        Log.Information(">>> GMAO: Web application starting on http://localhost:6001");
        await app.RunAsync("http://localhost:6001");
    }
    catch (IOException ioEx) when (ioEx.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase) || ioEx.Message.Contains("port", StringComparison.OrdinalIgnoreCase) || ioEx.Message.Contains("bind", StringComparison.OrdinalIgnoreCase))
    {
        Log.Warning("Port 6001 is already in use. Attempting fallback to a random available port...");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[AVERTISSEMENT] : Le port 6001 est déjà utilisé.");
        Console.WriteLine("L'application est probablement déjà lancée en arrière-plan.");
        Console.WriteLine("Lancement sur un port alternatif pour éviter les conflits.\n");
        Console.ForegroundColor = ConsoleColor.Gray;

        // Attempt fallback to any free port
        await app.RunAsync("http://localhost:0");
    }
}
catch (Exception ex)
{
    var crashLog = $"[FATAL ERROR {DateTime.Now}]\n{ex.ToString()}\n\nInner Exception: {ex.InnerException?.ToString()}";
    File.WriteAllText("crash_log.txt", crashLog);
    
    Log.Fatal(ex, "Application terminated unexpectedly");
    
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n====================================================");
    Console.WriteLine("FATAL ERROR - APPLICATION CRASHED");
    Console.WriteLine("====================================================");
    Console.WriteLine(ex.Message);
    Console.WriteLine("\nDetails saved to crash_log.txt");
    Console.WriteLine("\nAppuyez sur une touche pour fermer...");
    Console.ForegroundColor = ConsoleColor.Gray;
    
    if (!Console.IsInputRedirected)
    {
        Console.ReadKey();
    }
}
finally
{
    Log.CloseAndFlush();
}
