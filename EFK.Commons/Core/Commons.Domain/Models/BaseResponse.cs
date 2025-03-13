namespace Commons.Domain.Models
{
    public class BaseResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
