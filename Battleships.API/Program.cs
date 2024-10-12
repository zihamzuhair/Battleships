using Battleships.Services;
using Battleships.Services.IService;

using Battleships.DAL.DbFactory;
using Battleships.DAL.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContextFactory.
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();

// Register application services.
builder.Services.AddScoped(provider =>
{
    var factory = provider.GetRequiredService<IDbContextFactory>();
    return factory.CreateDbContext();
});

// Add services to the DI container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<Random>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();