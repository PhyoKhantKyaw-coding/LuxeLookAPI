namespace LuxeLookAPI.DTO
{
    public class AddBookingDto
    {
        public Guid DoctorId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
