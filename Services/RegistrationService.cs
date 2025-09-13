using EventSphere.Data;
using EventSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly EventSphereContext _context;
        private readonly IQRCodeService _qrCodeService;

        public RegistrationService(EventSphereContext context, IQRCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }

        public async Task<Registration?> GetRegistrationAsync(int eventId, string userId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
        }

        public async Task<Registration> RegisterForEventAsync(int eventId, string userId)
        {
            var existingRegistration = await GetRegistrationAsync(eventId, userId);
            if (existingRegistration != null)
            {
                throw new InvalidOperationException("User is already registered for this event.");
            }

            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            if (eventModel.CurrentRegistrations >= eventModel.MaxCapacity)
            {
                throw new InvalidOperationException("Event is full.");
            }

            var registration = new Registration
            {
                EventId = eventId,
                UserId = userId,
                Status = RegistrationStatus.Registered,
                RegistrationDate = DateTime.Now,
                QRCode = await _qrCodeService.GenerateQRCodeAsync(eventId, userId)
            };

            _context.Registrations.Add(registration);
            
            // Update event registration count
            eventModel.CurrentRegistrations++;
            
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task<bool> CancelRegistrationAsync(int eventId, string userId, string? reason = null)
        {
            var registration = await GetRegistrationAsync(eventId, userId);
            if (registration == null) return false;

            registration.Status = RegistrationStatus.Cancelled;
            registration.CancellationDate = DateTime.Now;
            registration.CancellationReason = reason;

            // Update event registration count
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel != null)
            {
                eventModel.CurrentRegistrations = Math.Max(0, eventModel.CurrentRegistrations - 1);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAttendanceAsync(int eventId, string userId)
        {
            var registration = await GetRegistrationAsync(eventId, userId);
            if (registration == null) return false;

            registration.AttendanceMarked = true;
            registration.AttendanceDate = DateTime.Now;
            registration.Status = RegistrationStatus.Attended;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Registration>> GetUserRegistrationsAsync(string userId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId)
        {
            return await _context.Registrations
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderBy(r => r.RegistrationDate)
                .ToListAsync();
        }

        public async Task<bool> IsUserRegisteredAsync(int eventId, string userId)
        {
            return await _context.Registrations
                .AnyAsync(r => r.EventId == eventId && r.UserId == userId && r.Status == RegistrationStatus.Registered);
        }

        public async Task<bool> CanUserRegisterAsync(int eventId, string userId)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null || eventModel.Status != EventStatus.Approved) return false;

            var isRegistered = await IsUserRegisteredAsync(eventId, userId);
            if (isRegistered) return false;

            return eventModel.CurrentRegistrations < eventModel.MaxCapacity;
        }

        public async Task<string> GenerateQRCodeAsync(int eventId, string userId)
        {
            return await _qrCodeService.GenerateQRCodeAsync(eventId, userId);
        }

        public async Task<bool> ValidateQRCodeAsync(string qrCode)
        {
            return await _qrCodeService.ValidateQRCodeAsync(qrCode);
        }
    }
}

