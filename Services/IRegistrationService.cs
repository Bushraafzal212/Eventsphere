using EventSphere.Models;

namespace EventSphere.Services
{
    public interface IRegistrationService
    {
        Task<Registration?> GetRegistrationAsync(int eventId, string userId);
        Task<Registration> RegisterForEventAsync(int eventId, string userId);
        Task<bool> CancelRegistrationAsync(int eventId, string userId, string? reason = null);
        Task<bool> MarkAttendanceAsync(int eventId, string userId);
        Task<IEnumerable<Registration>> GetUserRegistrationsAsync(string userId);
        Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId);
        Task<bool> IsUserRegisteredAsync(int eventId, string userId);
        Task<bool> CanUserRegisterAsync(int eventId, string userId);
        Task<string> GenerateQRCodeAsync(int eventId, string userId);
        Task<bool> ValidateQRCodeAsync(string qrCode);
    }
}

