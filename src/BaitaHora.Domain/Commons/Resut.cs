using System.Net;

namespace BaitaHora.Domain.Commons
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public string? Title { get; }
        public string? Error { get; }
        public HttpStatusCode Status { get; }

        protected Result(bool isSuccess, HttpStatusCode status, string? title, string? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Success result cannot have an error message.");
            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            Status = status;
            Title = title;
            Error = error;
        }

        public static Result Ok(string? title = null)
            => new(true, HttpStatusCode.OK, title, null);

        public static Result NoContent(string? title = null)
            => new(true, HttpStatusCode.NoContent, title, null);

        public static Result Created(string? title = null)
            => new(true, HttpStatusCode.Created, title, null);

        public static Result BadRequest(string message, string? title = null)
            => new(false, HttpStatusCode.BadRequest, title ?? "Requisição inválida", message);

        public static Result Conflict(string message, string? title = null)
            => new(false, HttpStatusCode.Conflict, title ?? "Conflito", message);

        public static Result Unauthorized(string message = "Não autorizado")
            => new(false, HttpStatusCode.Unauthorized, "Não autorizado", message);

        public static Result Forbidden(string message = "Acesso negado")
            => new(false, HttpStatusCode.Forbidden, "Acesso negado", message);

        public static Result NotFound(string message, string? title = null)
            => new(false, HttpStatusCode.NotFound, title ?? "Não encontrado", message);

        public static Result ServerError(string message = "Erro interno")
            => new(false, HttpStatusCode.InternalServerError, "Erro no servidor", message);

        public static Result Success() => Ok();
        public static Result Failure(string error) => BadRequest(error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, HttpStatusCode status, string? title, string? error)
            : base(isSuccess, status, title, error)
        {
            Value = value;
        }

        public static Result<T> Ok(T value, string? title = null)
            => new(true, value, HttpStatusCode.OK, title, null);

        public static Result<T> Created(T value, string? title = null)
            => new(true, value, HttpStatusCode.Created, title, null);

        public new static Result<T> BadRequest(string message, string? title = null)
            => new(false, default, HttpStatusCode.BadRequest, title ?? "Requisição inválida", message);

        public new static Result<T> Conflict(string message, string? title = null)
            => new(false, default, HttpStatusCode.Conflict, title ?? "Conflito", message);

        public new static Result<T> Unauthorized(string message = "Não autorizado")
            => new(false, default, HttpStatusCode.Unauthorized, "Não autorizado", message);

        public new static Result<T> Forbidden(string message = "Acesso negado")
            => new(false, default, HttpStatusCode.Forbidden, "Acesso negado", message);

        public new static Result<T> NotFound(string message, string? title = null)
            => new(false, default, HttpStatusCode.NotFound, title ?? "Não encontrado", message);

        public new static Result<T> ServerError(string message = "Erro interno")
            => new(false, default, HttpStatusCode.InternalServerError, "Erro no servidor", message);

        public static Result<T> Success(T value) => Ok(value);
        public new static Result<T> Failure(string error) => BadRequest(error);
    }
}