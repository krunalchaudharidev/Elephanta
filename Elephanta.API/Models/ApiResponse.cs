namespace Elephanta.API.Models
{
    // Standard API response wrapper used by controllers.
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        public ApiResponse()
        {
        }

        public ApiResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
