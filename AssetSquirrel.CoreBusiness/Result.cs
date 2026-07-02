namespace AssetSquirrel.CoreBusiness
{
    public class Result<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

        public static Result<T> Ok(T? data = default, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static Result<T> Fail(string message, T? data = default) =>
            new() { Success = false, Message = message, Data = data };

        public Result<TOut> Select<TOut>(Func<T, TOut> map) =>
            new()
            {
                Success = Success,
                Message = Message,
                Data = Data is not null ? map(Data) : default
            };
    }
}
