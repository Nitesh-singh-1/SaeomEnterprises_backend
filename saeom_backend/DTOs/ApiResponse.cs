namespace saeom_backend.DTOs
{
    public class ApiResponse<T>
    {
        public int response_code { get; set; }
        public string response_message { get; set; } = string.Empty;
        public T? data { get; set; }
    }
}
