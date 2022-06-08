using Hangfire;
using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<AchievementService>()
    .AddScoped<GameService>()
    .AddScoped<UserService>()
    .AddScoped<BetService>()
    .AddScoped<EloService>()

    .AddHangfire(x => x.UseInMemoryStorage())
    .AddHangfireServer()

    .AddDbContext<PerudoBotDbContext>()
    .AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerudoBotDbContext>();
    //db.Database.EnsureDeleted();
    db.Database.Migrate();

    var hf = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
    hf.Enqueue(() => Console.WriteLine("Hello, world!"));
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs");

app.MapControllers();

app.Run();
