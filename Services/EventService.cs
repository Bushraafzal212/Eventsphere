using EventSphere.Data;
using EventSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Services
{
    public class EventService : IEventService
    {
        private readonly EventSphereContext _context;

        public EventService(EventSphereContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved && e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPastEventsAsync()
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved && e.EventDate < DateTime.Today)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category)
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved && e.Category == category)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDepartmentAsync(string department)
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved && e.OrganizingDepartment == department)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
        {
            return await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved && 
                           (e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm)))
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Registrations)
                .Include(e => e.Media)
                .Include(e => e.Reviews)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateEventAsync(Event eventModel, string organizerId)
        {
            eventModel.CreatedAt = DateTime.Now;
            eventModel.UpdatedAt = DateTime.Now;
            eventModel.Status = EventStatus.Pending;
            eventModel.CurrentRegistrations = 0;

            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event> CreateEventAsync(Event eventModel)
        {
            eventModel.CreatedAt = DateTime.Now;
            eventModel.UpdatedAt = DateTime.Now;
            eventModel.Status = EventStatus.Pending;
            eventModel.CurrentRegistrations = 0;

            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event> UpdateEventAsync(Event eventModel)
        {
            eventModel.UpdatedAt = DateTime.Now;
            _context.Events.Update(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null) return false;

            eventModel.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveEventAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null) return false;

            eventModel.Status = EventStatus.Approved;
            eventModel.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectEventAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null) return false;

            eventModel.Status = EventStatus.Rejected;
            eventModel.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId)
        {
            return await _context.Events
                .Where(e => e.OrganizerEmail == organizerId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPendingEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.Pending)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsEventFullAsync(int eventId)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null) return true;

            return eventModel.CurrentRegistrations >= eventModel.MaxCapacity;
        }

        public async Task<int> GetAvailableSlotsAsync(int eventId)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null) return 0;

            return Math.Max(0, eventModel.MaxCapacity - eventModel.CurrentRegistrations);
        }
    }
}

