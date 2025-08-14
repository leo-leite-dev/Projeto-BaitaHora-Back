using BaitaHora.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Helpers
{
    public static class ApiResponseHelper
    {
        private static readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("ApiResponseHelper");

        public static ApiResponse<string> CreateSuccess(string message)
        {
            return new ApiResponse<string>
            {
                Message = message,
                Data = message
            };
        }

        public static ApiResponse<T> CreateSuccess<T>(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<object> CreateEmptySuccess(string message)
        {
            return new ApiResponse<object>
            {
                Message = message,
                Data = null!
            };
        }

        public static IActionResult CreateError(string message, string? detail = null, int statusCode = 500)
        {
            _logger.LogError("Erro: {Message}, Detalhes: {Detail}, Status Code: {StatusCode}", message, detail, statusCode);
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message,
                Detail = detail,
                StatusCode = statusCode
            };

            return new JsonResult(errorResponse)
            {
                StatusCode = statusCode
            };
        }
    }
}