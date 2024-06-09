using Domain.Common;
using Domain.Enums;

namespace Server.Common;

public abstract class BaseController
{
    protected static BaseResponse Ok(object value) => new(value);
    protected static BaseResponse BadRequest(params string[] errors) => new (ResponseStatus.BadRequest, errors);
    protected static BaseResponse Forbidden(params string[] errors) => new (ResponseStatus.Forbidden, errors);
    protected static BaseResponse InternalError(params string[] errors) => new (ResponseStatus.InternalError, errors);
    protected static BaseResponse Unauthorized(params string[] errors) => new (ResponseStatus.Unauthorized, errors);
}