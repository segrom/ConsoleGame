using DataAccess.DbSets;
using DataAccess.DbSets.GameServers;
using DataAccess.DbSets.Users;

namespace DataAccess;

public class DbContext: IDbContext
{
    public IUserSet UserSet { get; }
    public IGameServerSet GameServerSet { get; }
    
    public DbContext(IUserSet userSet, IGameServerSet gameServerSet)
    {
        UserSet = userSet;
        GameServerSet = gameServerSet;
    }
    
}