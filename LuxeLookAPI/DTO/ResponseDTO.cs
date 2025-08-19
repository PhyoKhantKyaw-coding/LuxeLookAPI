namespace LuxeLookAPI.DTO
{
    public class ResponseDTO
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public object? Data { get; set; }
    }
}
