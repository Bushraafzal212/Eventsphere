using EventSphere.Data;
using EventSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EventSphereContext _context;

        public NotificationService(EventSphereContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateNotificationAsync(string title, string message, NotificationType type, string? targetRole = null, string? targetUserId = null, int? eventId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                TargetRole = targetRole,
                TargetUserId = targetUserId,
                EventId = eventId,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Event)
                .Where(n => n.IsActive && (n.TargetUserId == userId || n.TargetUserId == null))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Event)
                .Where(n => n.IsActive && !n.IsRead && (n.TargetUserId == userId || n.TargetUserId == null))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.TargetUserId == userId);

            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.TargetUserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SendEventNotificationAsync(int eventId, string title, string message, NotificationType type)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null) return;

            // Get all registered users for this event
            var registeredUsers = await _context.Registrations
                .Where(r => r.EventId == eventId && r.Status == RegistrationStatus.Registered)
                .Select(r => r.UserId)
                .ToListAsync();

            // Create notifications for each registered user
            foreach (var userId in registeredUsers)
            {
                await CreateNotificationAsync(title, message, type, null, userId, eventId);
            }
        }

        public async Task SendRegistrationNotificationAsync(int eventId, string userId, NotificationType type)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null) return;

            string title = type switch
            {
                NotificationType.RegistrationConfirmed => "Registration Confirmed",
                NotificationType.RegistrationCancelled => "Registration Cancelled",
                NotificationType.EventReminder => "Event Reminder",
                _ => "Event Update"
            };

            string message = type switch
            {
                NotificationType.RegistrationConfirmed => $"Your registration for '{eventModel.Title}' has been confirmed.",
                NotificationType.RegistrationCancelled => $"Your registration for '{eventModel.Title}' has been cancelled.",
                NotificationType.EventReminder => $"Reminder: '{eventModel.Title}' is scheduled for {eventModel.EventDate:MMM dd, yyyy} at {eventModel.Venue}.",
                _ => $"Update regarding '{eventModel.Title}'."
            };

            await CreateNotificationAsync(title, message, type, null, userId, eventId);
        }
    }
}

