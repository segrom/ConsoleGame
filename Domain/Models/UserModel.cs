using Domain.Common;

namespace Domain.Models;

public class UserModel: IModel
{
    public Guid Id { get; set; }
    public string Nickname { get; set; }
    public string PasswordHash { get; set; }
}