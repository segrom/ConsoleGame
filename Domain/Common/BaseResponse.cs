using Domain.Enums;

namespace Domain.Common;

public class BaseResponse
{
    public object? Value { get; set; }
    public ResponseStatus Status { get; set; }
    public string[]? Errors { get; set; }

    public BaseResponse(object value)
    {
        Value = value;
        Status = ResponseStatus.Ok;
        Errors = null;
    }
    
    public BaseResponse(ResponseStatus status, string[]? errors = null)
    {
        Status = status;
        Errors = errors;
    }
}