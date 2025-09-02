  // for DbContext
using LuxeLookAPI.DTO;  // for DTOs
using LuxeLookAPI.Models; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuxeLookAPI.Services
{
    public class BookingService
    {
        private readonly DataContext _context;
        private readonly CommonTokenReader _tokenReader;

        public BookingService(DataContext context, CommonTokenReader tokenReader)
        {
            _context = context;
            _tokenReader = tokenReader;
        }

        // Add Booking
        public async Task<bool>  AddBooking(AddBookingDto dto)
        {
            var (userId, _, _) = _tokenReader.GetUserFromContext();
            if (userId == null)
                return false;
            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                UserId = userId.Value ,
                Title = dto.Title,
                Description = dto.Description,
                BookingDate = dto.BookingDate
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return true;
        }

        // Get All Bookings for Logged-in User
        public List<BookingResponseDto> GetAllBookings()
        {
            var (userId, _, _) = _tokenReader.GetUserFromContext();

            var result = (from b in _context.Bookings
                          join d in _context.Doctors on b.DoctorId equals d.DoctorId
                          join u in _context.Users on b.UserId equals u.UserId // if you have Users table
                          where b.UserId == userId
                          select new BookingResponseDto
                          {
                              DoctorName = d.Name,
                              DoctorPhone = d.phoneNumber,
                              DoctorEmail = d.email,
                              StoreName = d.storeName,
                              UserName = u.UserName!,            // adjust property name based on your User entity
                              BookingStore = d.storeposition,   // I assume "storeposition" is booking store
                              BookingDate = b.BookingDate,
                              BookingDescription = b.Description
                          }).ToList();

            return result;
        }

        public Doctor AddDoctor(AddDoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Doctor name is required");

            var doctor = new Doctor
            {
                DoctorId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                storeposition = dto.StorePosition,
                storeName = dto.StoreName,
                phoneNumber = dto.PhoneNumber,
                email = dto.Email
            };

            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            return doctor;
        }

        // Get All Doctors
        public List<Doctor> GetAllDoctors()
        {
            return _context.Doctors.ToList();
        }
    }
}
