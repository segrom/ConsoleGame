using DataAccess.DbSets;
using DataAccess.DbSets.GameServers;
using DataAccess.DbSets.Users;

namespace DataAccess;

public interface IDbContext
{
    IGameServerSet GameServerSet { get; }
    IUserSet UserSet {get;}
}