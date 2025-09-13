using EventSphere.Models;

namespace EventSphere.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetPastEventsAsync();
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category);
        Task<IEnumerable<Event>> GetEventsByDepartmentAsync(string department);
        Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventModel, string organizerId);
        Task<Event> CreateEventAsync(Event eventModel);
        Task<Event> UpdateEventAsync(Event eventModel);
        Task<bool> DeleteEventAsync(int id);
        Task<bool> ApproveEventAsync(int id);
        Task<bool> RejectEventAsync(int id);
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId);
        Task<IEnumerable<Event>> GetPendingEventsAsync();
        Task<bool> IsEventFullAsync(int eventId);
        Task<int> GetAvailableSlotsAsync(int eventId);
    }
}

