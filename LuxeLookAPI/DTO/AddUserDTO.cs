namespace LuxeLookAPI.DTO;

public class AddUserDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? UserName { get; set; }
    public int? Age { get; set; }
    public string? RoleName { get; set; }
    public string? ProfileImageUrl { get; set; }
}
public class ResetPasswordDTO
{
    public string? Email { get; set; }
    public string? NewPassword { get; set; }
}

