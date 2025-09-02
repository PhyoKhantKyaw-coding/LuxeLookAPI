namespace LuxeLookAPI.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid DoctorId { get; set; }
        public Guid UserId { get; set; }
        public DateTime BookingDate { get; set; }
        
    }
}
