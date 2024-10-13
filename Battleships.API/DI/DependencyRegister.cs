using Battleships.DAL.UnitOfWork;
using Battleships.Services.IServices;
using Battleships.Services;
using Battleships.DAL.DbFactory;

namespace Battleships.API.DI
{
    public static class DependencyRegister
    {
        public static IServiceCollection DependancyInjection(this IServiceCollection service)
        {
            // Register the DbContextFactory.
            service.AddSingleton<IDbContextFactory, DbContextFactory>();

            // Register application services.
            service.AddScoped(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory>();
                return factory.CreateDbContext();
            });
                        
            // Register application services.
            service.AddSingleton<Random>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IGameService, GameService>();
                
            return service;
        }
    }
}