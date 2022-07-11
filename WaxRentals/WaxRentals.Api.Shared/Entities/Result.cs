#nullable disable

namespace WaxRentals.Api.Entities
{
    public class Result<T>
    {

        public bool Success { get; set; }
        public string Error { get; set; }
        public T Value { get; set; }

        public static Result<T> Succeed(T value)
        {
            return new Result<T> { Success = true, Value = value };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T> { Error = error };
        }

    }
}
