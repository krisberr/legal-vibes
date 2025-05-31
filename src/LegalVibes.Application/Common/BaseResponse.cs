namespace LegalVibes.Application.Common;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public static BaseResponse<T> SuccessResult(T data, string? message = null)
    {
        return new BaseResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static BaseResponse<T> ErrorResult(string error)
    {
        return new BaseResponse<T>
        {
            Success = false,
            Error = error
        };
    }
} 