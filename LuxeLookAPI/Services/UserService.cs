using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using LuxeLookAPI.Share;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LuxeLookAPI.Services;

public class UserService
{
    private readonly DataContext _context;
    private readonly CommonTokenGenerator _tokenGenerator;

    public UserService(DataContext context, CommonTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    // Add new user (Register)
    public async Task<UserModel> AddUserAsync(AddUserDTO dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email already exists.");

        CommonAuthentication.CreatePasswordHash(dto.Password!, out byte[] passwordHash);

        // Generate OTP
        var otpCode = GenerateOTP();

        var user = new UserModel
        {
            UserId = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = Convert.ToBase64String(passwordHash),
            UserName = dto.UserName,
            Age = dto.Age,
            RoleName = dto.RoleName ?? "User",
            ProfileImageUrl = dto.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            ActiveFlag = true,
            Status = "N", // Not verified
            OTP = otpCode,
            OTP_Exp = DateTime.Now.AddMinutes(5)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Send OTP email
        bool emailSent = SendOTPEmail(user.Email!, user.UserName ?? "User", otpCode);
        if (!emailSent)
            throw new Exception("Failed to send OTP email.");

        return user;
    }
    private static string GenerateOTP()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private static bool SendOTPEmail(string toEmail, string userName, string otpCode)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("ddd420698@gmail.com");
            mail.To.Add(toEmail);
            mail.Subject = "Your OTP Code from Phyo Khant Kyaw";


            string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto; background-color: #f9f9f9;'>
                <h2 style='color: #007bff; text-align: center;'>Your OTP Code</h2>
                <p style='font-size: 16px; color: #333;'>Dear <strong>{userName}</strong>,</p>
                <p style='font-size: 16px; color: #333;'>Your One-Time Password (OTP) for verification is:</p>
                <p style='font-size: 24px; font-weight: bold; color: #28a745; text-align: center; padding: 10px; border: 2px dashed #28a745; display: inline-block;'>{otpCode}</p>
                <p style='font-size: 14px; color: #ff0000; text-align: center;'>This OTP will expire in 5 minutes.</p>
               
                <br>
                <p style='font-size: 14px; color: #666; text-align: center;'>Best regards,</p>
                <p style='font-size: 14px; color: #666; text-align: center;'><strong>Retail</strong></p>
            </div>";

            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                //Credentials = new NetworkCredential("acetravelagency.net@gmail.com", "tkdm txbp kkaa lagm"),
                Credentials = new NetworkCredential("ddd420698@gmail.com", "ilma sfqt uhbm wubu"),
                EnableSsl = true
            };

            smtpClient.Send(mail);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> VerifyEmail(string email, string otp)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.OTP == otp && x.Status == "N");

        if (user == null)
        {
            return "Invalid OTP or email.";
        }

        if (user.OTP_Exp < DateTime.Now)
        {
            return "OTP expired.";
        }

        user.Status = "Y";
        user.OTP = null;
        user.OTP_Exp =null;
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user); try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Error saving user: {ex.InnerException?.Message ?? ex.Message}");
        }


        return "Email verified successfully.";
    }
    public async Task<ResponseDTO> ResentOTP(string email)
    {
        var model = new ResponseDTO();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.Status == "N");

        if (user == null)
        {
            model.Status = APIStatus.Error;
            model.Message = "User not found or already verified.";
            return model;
        }

        var otpCode = GenerateOTP();
        user.OTP = otpCode;
        user.OTP_Exp = DateTime.Now.AddMinutes(5);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        bool emailSent = SendOTPEmail(user.Email!, user.UserName ?? "User", otpCode);
        if (!emailSent)
        {
            model.Message = "Failed to send OTP email.";
            return model;
        }

        model.Message = "New OTP sent to email.";
        return model;
    }

    // Get user by ID
    public async Task<UserModel?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    // Get all users
    public async Task<List<UserModel>> GetAllUser()
    {
        return await _context.Users.ToListAsync();
    }

    // Login
    public async Task<LoginResponseDTO?> Loginweb(LoginDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Status == "Y" && u.ActiveFlag);
        if (user == null)
            return null;

        var storedHash = Convert.FromBase64String(user.PasswordHash!);
        if (!CommonAuthentication.VerifyPasswordHash(dto.Password!, storedHash))
            return null;

        // Generate JWT
        var token = _tokenGenerator.Create(user);

        return new LoginResponseDTO
        {
            Token = token,
            UserName = user.UserName,
            RoleName = user.RoleName
        };
    }
}
