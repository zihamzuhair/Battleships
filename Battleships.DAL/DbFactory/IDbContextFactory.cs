using Battleships.DAL.Context;

namespace Battleships.DAL.DbFactory
{
    public interface IDbContextFactory
    {
        BattleshipDbContext CreateDbContext();
    }
}
