using LuxeLookAPI.DTO;
using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // POST: api/Booking/add
        [HttpPost("add")]
        public IActionResult AddBooking( AddBookingDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Booking data is required." });

            try
            {
                var booking = _bookingService.AddBooking(request);
                return Ok(new { Status = 200, Success = true, Data = booking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

        // GET: api/Booking/all
        [HttpGet("all")]
        public IActionResult GetAllBookings()
        {
            try
            {
                var bookings = _bookingService.GetAllBookings();
                return Ok(new { Status = 200, Success = true, Data = bookings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

        // GET: api/Booking/doctors
        [HttpGet("doctors")]
        public IActionResult GetAllDoctors()
        {
            try
            {
                var doctors = _bookingService.GetAllDoctors();
                return Ok(new { Status = 200, Success = true, Data = doctors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
        // POST: api/Booking/add-doctor
        [HttpPost("add-doctor")]
        public IActionResult AddDoctor([FromBody] AddDoctorDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Doctor name is required." });

            try
            {
                var doctor = _bookingService.AddDoctor(request);
                return Ok(new { Status = 200, Success = true, Data = doctor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

    }
}
