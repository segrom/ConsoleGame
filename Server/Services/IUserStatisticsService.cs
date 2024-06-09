namespace Server.Services;

public interface IUserStatisticsService
{
    void UserIncrementKillScore(Guid userId);
}