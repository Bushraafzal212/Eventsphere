using EventSphere.Models;

namespace EventSphere.Services
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(string title, string message, NotificationType type, string? targetRole = null, string? targetUserId = null, int? eventId = null);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<bool> MarkAsReadAsync(int notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task SendEventNotificationAsync(int eventId, string title, string message, NotificationType type);
        Task SendRegistrationNotificationAsync(int eventId, string userId, NotificationType type);
    }
}

