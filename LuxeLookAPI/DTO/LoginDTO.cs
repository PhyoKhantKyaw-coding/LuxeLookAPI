namespace LuxeLookAPI.DTO
{
    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public string? UserName { get; set; }
        public string? RoleName { get; set; }
    }
}
