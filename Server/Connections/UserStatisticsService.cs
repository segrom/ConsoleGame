using DataAccess;
using Server.Services;

namespace Server.Connections;

public class UserStatisticsService: IUserStatisticsService
{
    private IDbContext _context;
    public UserStatisticsService(IDbContext context)
    {
        _context = context;
    }

    public async void UserIncrementKillScore(Guid userId)
    {
        var user = await _context.UserSet.GetAsync(userId);
        if (user is null) throw new Exception("User not found");
        user.Kills++;
        await _context.UserSet.UpdateAsync(user);
    }
}