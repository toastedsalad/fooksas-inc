public class Result<T> {
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure { get; }

    private Result(T value) {
        Value = value;
        IsSuccess = true;
        IsFailure = false;
        Error = null;
    }

    private Result(string error) {
        Value = default;
        IsSuccess = false;
        IsFailure = true;
        Error = error;
    }

    public static Result<T> Ok(T value) => new Result<T>(value);
    public static Result<T> Fail(string error) => new Result<T>(error);
}

