using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EventSphere.Models;
using EventSphere.Services;
using EventSphere.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;
        private readonly INotificationService _notificationService;
        private readonly IImageService _imageService;
        private readonly EventSphereContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(
            IEventService eventService,
            IRegistrationService registrationService,
            INotificationService notificationService,
            IImageService imageService,
            EventSphereContext context,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _registrationService = registrationService;
            _notificationService = notificationService;
            _imageService = imageService;
            _context = context;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index(string? category, string? department, string? searchTerm)
        {
            ViewData["CurrentCategory"] = category;
            ViewData["CurrentDepartment"] = department;
            ViewData["CurrentSearch"] = searchTerm;

            IEnumerable<Event> events;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                events = await _eventService.SearchEventsAsync(searchTerm);
            }
            else if (!string.IsNullOrEmpty(category))
            {
                events = await _eventService.GetEventsByCategoryAsync(category);
            }
            else if (!string.IsNullOrEmpty(department))
            {
                events = await _eventService.GetEventsByDepartmentAsync(department);
            }
            else
            {
                events = await _eventService.GetUpcomingEventsAsync();
            }

            // Get unique categories and departments for filter dropdowns
            var categories = await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved)
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var departments = await _context.Events
                .Where(e => e.IsActive && e.Status == EventStatus.Approved)
                .Select(e => e.OrganizingDepartment)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            ViewData["Categories"] = categories;
            ViewData["Departments"] = departments;
            ViewData["ImageService"] = _imageService;

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _eventService.GetEventByIdAsync(id.Value);
            if (eventModel == null)
            {
                return NotFound();
            }

            // Check if user is registered
            var currentUser = await _userManager.GetUserAsync(User);
            bool isRegistered = false;
            bool canRegister = false;

            if (currentUser != null)
            {
                isRegistered = await _registrationService.IsUserRegisteredAsync(id.Value, currentUser.Id);
                canRegister = await _registrationService.CanUserRegisterAsync(id.Value, currentUser.Id);
            }

            ViewData["IsRegistered"] = isRegistered;
            ViewData["CanRegister"] = canRegister;
            ViewData["AvailableSlots"] = await _eventService.GetAvailableSlotsAsync(id.Value);
            ViewData["ImageService"] = _imageService;

            return View(eventModel);
        }

        // GET: Events/Create
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,EventType,Venue,EventDate,StartTime,EndTime,OrganizingDepartment,OrganizerName,OrganizerEmail,MaxCapacity,CertificateFee")] Event eventModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    await _eventService.CreateEventAsync(eventModel, currentUser.Id);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(eventModel);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _eventService.GetEventByIdAsync(id.Value);
            if (eventModel == null)
            {
                return NotFound();
            }

            // Check if user can edit this event
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || 
                (currentUser.Role != UserRole.Admin && eventModel.OrganizerEmail != currentUser.Email))
            {
                return Forbid();
            }

            return View(eventModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,EventType,Venue,EventDate,StartTime,EndTime,OrganizingDepartment,OrganizerName,OrganizerEmail,MaxCapacity,CertificateFee")] Event eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _eventService.UpdateEventAsync(eventModel);
                    
                    // Send notification to registered users
                    await _notificationService.SendEventNotificationAsync(
                        eventModel.Id, 
                        "Event Updated", 
                        $"The event '{eventModel.Title}' has been updated.", 
                        NotificationType.EventUpdated);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EventExists(eventModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _eventService.GetEventByIdAsync(id.Value);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Events/Register/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Register(int eventId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                await _registrationService.RegisterForEventAsync(eventId, currentUser.Id);
                
                // Send notification
                await _notificationService.SendRegistrationNotificationAsync(
                    eventId, currentUser.Id, NotificationType.RegistrationConfirmed);

                TempData["SuccessMessage"] = "Successfully registered for the event!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        // POST: Events/CancelRegistration/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CancelRegistration(int eventId, string? reason)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var success = await _registrationService.CancelRegistrationAsync(eventId, currentUser.Id, reason);
            if (success)
            {
                // Send notification
                await _notificationService.SendRegistrationNotificationAsync(
                    eventId, currentUser.Id, NotificationType.RegistrationCancelled);

                TempData["SuccessMessage"] = "Registration cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel registration.";
            }

            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        // GET: Events/Approve/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _eventService.ApproveEventAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Event approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve event.";
            }

            return RedirectToAction(nameof(PendingEvents));
        }

        // GET: Events/Reject/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _eventService.RejectEventAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Event rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject event.";
            }

            return RedirectToAction(nameof(PendingEvents));
        }

        // GET: Events/PendingEvents
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingEvents()
        {
            var pendingEvents = await _eventService.GetPendingEventsAsync();
            return View(pendingEvents);
        }

        // GET: Events/MyEvents
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> MyEvents()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var myEvents = await _eventService.GetEventsByOrganizerAsync(currentUser.Email);
            return View(myEvents);
        }

        // GET: Events/GetUpcomingEventsForDropdown
        [HttpGet]
        public async Task<IActionResult> GetUpcomingEventsForDropdown()
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync();
                var upcomingEvents = events.Take(5).Select(e => new
                {
                    id = e.Id,
                    title = e.Title,
                    category = e.Category,
                    eventType = e.EventType,
                    eventDate = e.EventDate.ToString("MMM dd, yyyy"),
                    startTime = e.StartTime.ToString(@"hh\:mm"),
                    venue = e.Venue,
                    description = e.Description.Length > 100 ? e.Description.Substring(0, 100) + "..." : e.Description,
                    bannerImageUrl = e.BannerImageUrl,
                    currentRegistrations = e.CurrentRegistrations,
                    maxCapacity = e.MaxCapacity
                }).ToList();

                return Json(upcomingEvents);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Failed to load events" });
            }
        }

        private async Task<bool> EventExists(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            return eventModel != null;
        }

        // GET: Events/RefreshEvents
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefreshEvents()
        {
            try
            {
                // Log the refresh attempt
                Console.WriteLine("Starting event refresh...");
                
                // Clear existing events
                var existingEvents = await _context.Events.ToListAsync();
                Console.WriteLine($"Found {existingEvents.Count} existing events to remove");
                
                if (existingEvents.Any())
                {
                    _context.Events.RemoveRange(existingEvents);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Existing events removed");
                }
                
                // Seed new events
                await EventDataSeeder.SeedEventsAsync(_context);
                Console.WriteLine("New events seeded");
                
                TempData["SuccessMessage"] = $"Events have been refreshed successfully! Removed {existingEvents.Count} old events and added 7 new events.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing events: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to refresh events: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

    }
}

