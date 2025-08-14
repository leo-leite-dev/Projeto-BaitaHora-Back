namespace BaitaHora.Domain.Commons
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Success result cannot have an error message.");
            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);

        public static Result Failure(string error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);

        public static new Result<T> Failure(string error) => new Result<T>(false, default, error);
    }
}