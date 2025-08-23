using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LuxeLookAPI.Share;

public class ImageUploader
{
    private readonly string _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        // Ensure directory exists
        if (!Directory.Exists(_imageFolder))
            Directory.CreateDirectory(_imageFolder);

        // Create unique file name
        var fileName = file.FileName;

        // Full path
        var filePath = Path.Combine(_imageFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative URL (like "images/freshglow_gel.jpg")
        return $"images/{fileName}";
    }
}
