using MyMVCProject.Api.Global.Errors;

namespace MyMVCProject.Api.Global;

public class Result<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; }
    public AppError? Error { get; set; }

    private Result(T? data, AppError? error, bool isSuccess)
    {
        Data = data;
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result<T> Success(T data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        return new Result<T>(data, null, true);
    }
    
    public static Result<T> Failure(AppError error)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));
        return new Result<T>(default, error, false);
    }
}