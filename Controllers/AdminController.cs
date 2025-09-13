using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventSphere.Models;
using EventSphere.Services;
using Microsoft.AspNetCore.Identity;

namespace EventSphere.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IEventService eventService, IImageService imageService, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
        {
            _eventService = eventService;
            _imageService = imageService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Admin/Events
        public async Task<IActionResult> Events()
        {
            var events = await _eventService.GetAllEventsAsync();
            ViewData["ImageService"] = _imageService;
            return View(events);
        }

        // GET: Admin/Events/Create
        public IActionResult CreateEvent()
        {
            ViewBag.Categories = Enum.GetValues<EventCategory>();
            ViewBag.Statuses = Enum.GetValues<EventStatus>();
            return View();
        }

        // POST: Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        eventModel.OrganizerEmail = currentUser.Email;
                        eventModel.OrganizerName = $"{currentUser.FirstName} {currentUser.LastName}";
                        eventModel.CreatedAt = DateTime.Now;
                        eventModel.UpdatedAt = DateTime.Now;
                    }

                    await _eventService.CreateEventAsync(eventModel);
                    TempData["SuccessMessage"] = "Event created successfully!";
                    return RedirectToAction(nameof(Events));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating event");
                    TempData["ErrorMessage"] = "An error occurred while creating the event.";
                }
            }

            ViewBag.Categories = Enum.GetValues<EventCategory>();
            ViewBag.Statuses = Enum.GetValues<EventStatus>();
            return View(eventModel);
        }

        // GET: Admin/Events/Edit/5
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            ViewBag.Categories = Enum.GetValues<EventCategory>();
            ViewBag.Statuses = Enum.GetValues<EventStatus>();
            return View(eventModel);
        }

        // POST: Admin/Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, Event eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    eventModel.UpdatedAt = DateTime.Now;
                    await _eventService.UpdateEventAsync(eventModel);
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Events));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating event");
                    TempData["ErrorMessage"] = "An error occurred while updating the event.";
                }
            }

            ViewBag.Categories = Enum.GetValues<EventCategory>();
            ViewBag.Statuses = Enum.GetValues<EventStatus>();
            return View(eventModel);
        }

        // GET: Admin/Events/Delete/5
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Admin/Events/Delete/5
        [HttpPost, ActionName("DeleteEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                TempData["ErrorMessage"] = "An error occurred while deleting the event.";
            }

            return RedirectToAction(nameof(Events));
        }

        // GET: Admin/Events/Details/5
        public async Task<IActionResult> EventDetails(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Admin/Events/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            try
            {
                var eventModel = await _eventService.GetEventByIdAsync(id);
                if (eventModel != null)
                {
                    eventModel.Status = EventStatus.Approved;
                    eventModel.UpdatedAt = DateTime.Now;
                    await _eventService.UpdateEventAsync(eventModel);
                    TempData["SuccessMessage"] = "Event approved successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving event");
                TempData["ErrorMessage"] = "An error occurred while approving the event.";
            }

            return RedirectToAction(nameof(Events));
        }

        // POST: Admin/Events/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEvent(int id)
        {
            try
            {
                var eventModel = await _eventService.GetEventByIdAsync(id);
                if (eventModel != null)
                {
                    eventModel.Status = EventStatus.Rejected;
                    eventModel.UpdatedAt = DateTime.Now;
                    await _eventService.UpdateEventAsync(eventModel);
                    TempData["SuccessMessage"] = "Event rejected successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting event");
                TempData["ErrorMessage"] = "An error occurred while rejecting the event.";
            }

            return RedirectToAction(nameof(Events));
        }
    }
}
