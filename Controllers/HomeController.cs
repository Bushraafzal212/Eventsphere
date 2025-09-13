using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EventSphere.Models;
using EventSphere.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EventSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEventService eventService, IImageService imageService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _eventService = eventService;
            _imageService = imageService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured events for the homepage
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            var featuredEvents = upcomingEvents.Take(6).ToList();

            ViewData["FeaturedEvents"] = featuredEvents;
            ViewData["TotalEvents"] = upcomingEvents.Count();
            ViewData["ImageService"] = _imageService;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Sitemap()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["UserRole"] = currentUser.Role;
            ViewData["UserName"] = $"{currentUser.FirstName} {currentUser.LastName}";

            // Get user-specific data based on role
            switch (currentUser.Role)
            {
                case UserRole.Participant:
                    return RedirectToAction("ParticipantDashboard");
                case UserRole.Organizer:
                    return RedirectToAction("OrganizerDashboard");
                case UserRole.Admin:
                    return RedirectToAction("AdminDashboard");
                default:
                    return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> ParticipantDashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get upcoming events
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            ViewData["UpcomingEvents"] = upcomingEvents.Take(5);

            return View();
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> OrganizerDashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get organizer's events
            var myEvents = await _eventService.GetEventsByOrganizerAsync(currentUser.Email);
            ViewData["MyEvents"] = myEvents;

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get pending events for approval
            var pendingEvents = await _eventService.GetPendingEventsAsync();
            ViewData["PendingEvents"] = pendingEvents;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
