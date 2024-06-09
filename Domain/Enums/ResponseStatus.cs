namespace Domain.Enums;

public enum ResponseStatus
{
    Ok = 200,
    BadRequest = 400,
    NotFound = 404,
    Forbidden = 403,
    Unauthorized = 401,
    InternalError = 500,
}