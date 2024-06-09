using DataAccess.Common;
using Domain.Models;

namespace DataAccess.DbSets.Users;

public interface IUserSet: ICrudSet<UserModel>
{
    Task<UserModel?> GetUserByNicknameAsync(string requestNickname);
}