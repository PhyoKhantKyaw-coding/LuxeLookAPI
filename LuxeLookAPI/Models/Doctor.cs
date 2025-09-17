using System.ComponentModel.DataAnnotations.Schema;

namespace LuxeLookAPI.Models
{
    [Table("Doctor")]
    public class Doctor
    {
        public Guid DoctorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string storeposition { get; set; }
        public string storeName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
