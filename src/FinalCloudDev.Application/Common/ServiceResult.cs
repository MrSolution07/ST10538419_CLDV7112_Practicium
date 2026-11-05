namespace FinalCloudDev.Application.Common;

public sealed class ServiceResult<T>
{
    public bool Succeeded { get; private init; }

    public T? Value { get; private init; }

    public string? Error { get; private init; }

    public static ServiceResult<T> Ok(T value) => new()
    {
        Succeeded = true,
        Value = value
    };

    public static ServiceResult<T> Fail(string error) => new()
    {
        Succeeded = false,
        Error = error
    };
}
