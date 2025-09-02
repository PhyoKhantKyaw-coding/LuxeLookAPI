namespace LuxeLookAPI.DTO
{
    public class BookingResponseDto
    {
        public string DoctorName { get; set; }
        public string DoctorPhone { get; set; }
        public string DoctorEmail { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; }
        public string BookingStore { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDescription { get; set; }
    }
}
