using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Constants;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<AchievementService>()
    .AddScoped<GameService>()
    .AddScoped<UserService>()
    .AddScoped<BetService>()
    .AddScoped<EloService>()
    .AddScoped<ItemService>()
    .AddScoped<AuctionService>()
    .AddDbContext<PerudoBotDbContext>()
    .AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerudoBotDbContext>();
    //db.Database.EnsureDeleted();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
