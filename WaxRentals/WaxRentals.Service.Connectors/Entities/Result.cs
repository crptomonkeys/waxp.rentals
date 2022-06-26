namespace WaxRentals.Service.Shared.Entities
{
    public class Result
    {

        public bool Success { get; set; }
        public string? Error { get; set; }

        public static Result Succeed()
        {
            return new Result { Success = true };
        }

        public static Result Fail(string error)
        {
            return new Result { Error = error };
        }

    }

    public class Result<T> : Result
    {

        public T? Value { get; set; }

        public static Result<T> Succeed(T value)
        {
            return new Result<T> { Success = true, Value = value };
        }

        public static new Result<T> Fail(string error)
        {
            return new Result<T> { Error = error };
        }

    }
}
