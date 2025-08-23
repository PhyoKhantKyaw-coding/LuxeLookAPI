using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models;

[Table("tblUser")]
public class UserModel : CommonFields
{
    [Key]
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? OTP { get; set; }
    public string? Status { get; set; } = "N";
    public DateTime? OTP_Exp { get; set; }
    public string? PasswordHash { get; set; }
    public string? UserName { get; set; }
    public int? Age { get; set; }
    public string? RoleName { get; set; } // Admin, User, Delivery
    public string? ProfileImageUrl { get; set; }
}
