namespace BaitaHora.Api.Models
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public string? Detail { get; set; }
        public int StatusCode { get; set; }

        public ApiErrorResponse() { }

        public ApiErrorResponse(string message, string? detail = null)
        {
            Message = message;
            Detail = detail;
        }
    }
}