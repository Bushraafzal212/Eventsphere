using EventSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Data
{
    public static class EventDataSeeder
    {
        public static async Task SeedEventsAsync(EventSphereContext context)
        {
            // Always clear existing events to ensure fresh data
            var existingEvents = await context.Events.ToListAsync();
            if (existingEvents.Any())
            {
                context.Events.RemoveRange(existingEvents);
                await context.SaveChangesAsync();
            }

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Meet MDX Dubai at Aptech Qatar",
                    Description = "Join us for an exclusive meet and greet session with Middlesex University Dubai representatives at Aptech Qatar. Learn about MDX programs, admission requirements, and career opportunities. Get your questions answered directly by university officials.",
                    Category = "Career",
                    EventType = "Information Session",
                    Venue = "Aptech Qatar - Doha Campus",
                    EventDate = DateTime.Now.AddDays(8),
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    OrganizingDepartment = "Academic Affairs",
                    OrganizerName = "Dr. Sarah Ahmed",
                    OrganizerEmail = "sarah.ahmed@aptechqatar.com",
                    MaxCapacity = 100,
                    CurrentRegistrations = 65,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 0,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                },
                new Event
                {
                    Title = "Katara Tech Forum Workshop",
                    Description = "Explore the latest in technology and innovation at Katara Cultural Village. This comprehensive workshop covers emerging technologies, digital transformation, and career opportunities in the tech industry. Hands-on sessions and networking opportunities included.",
                    Category = "Academic",
                    EventType = "Workshop",
                    Venue = "Katara Cultural Village - Tech Hub",
                    EventDate = DateTime.Now.AddDays(15),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    OrganizingDepartment = "Technology Department",
                    OrganizerName = "Eng. Mohammed Al-Kuwari",
                    OrganizerEmail = "mohammed.alkuwari@aptechqatar.com",
                    MaxCapacity = 80,
                    CurrentRegistrations = 45,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 50.00m,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now.AddDays(-6)
                },
                new Event
                {
                    Title = "Career Quest & Graduation Ceremony",
                    Description = "Celebrate your academic achievements and explore career opportunities! This dual event features a graduation ceremony for completing students followed by a career fair with top employers from Qatar and the region. Dress code: Formal attire required.",
                    Category = "Career",
                    EventType = "Ceremony & Job Fair",
                    Venue = "Qatar National Convention Centre",
                    EventDate = DateTime.Now.AddDays(22),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    OrganizingDepartment = "Student Affairs",
                    OrganizerName = "Ms. Aisha Al-Sulaiti",
                    OrganizerEmail = "aisha.alsulaiti@aptechqatar.com",
                    MaxCapacity = 300,
                    CurrentRegistrations = 250,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 0,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                },
                new Event
                {
                    Title = "Techno Minds Competition",
                    Description = "Showcase your technical skills and innovative thinking! This coding and technology competition features challenges in programming, web development, mobile apps, and AI. Prizes for winners and recognition for all participants. Individual and team categories available.",
                    Category = "Academic",
                    EventType = "Competition",
                    Venue = "Aptech Qatar - Computer Lab",
                    EventDate = DateTime.Now.AddDays(18),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(20, 0, 0),
                    OrganizingDepartment = "Computer Science",
                    OrganizerName = "Prof. Khalid Al-Mahmoud",
                    OrganizerEmail = "khalid.almahmoud@aptechqatar.com",
                    MaxCapacity = 60,
                    CurrentRegistrations = 40,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 25.00m,
                    CreatedAt = DateTime.Now.AddDays(-12),
                    UpdatedAt = DateTime.Now.AddDays(-7)
                },
                new Event
                {
                    Title = "Bowling Tournament",
                    Description = "Strike up some fun at our annual bowling tournament! Open to all students and staff. Individual and team competitions with prizes for top scorers. Bowling equipment provided. Refreshments and awards ceremony included.",
                    Category = "Sports",
                    EventType = "Tournament",
                    Venue = "Qatar Bowling Center - Villaggio Mall",
                    EventDate = DateTime.Now.AddDays(25),
                    StartTime = new TimeSpan(15, 0, 0),
                    EndTime = new TimeSpan(21, 0, 0),
                    OrganizingDepartment = "Sports & Recreation",
                    OrganizerName = "Coach Nasser Al-Thani",
                    OrganizerEmail = "nasser.althani@aptechqatar.com",
                    MaxCapacity = 120,
                    CurrentRegistrations = 85,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 30.00m,
                    CreatedAt = DateTime.Now.AddDays(-8),
                    UpdatedAt = DateTime.Now.AddDays(-4)
                },
                new Event
                {
                    Title = "Desert Safari",
                    Description = "Experience the beauty of Qatar's desert landscape! Join us for an exciting desert safari adventure including dune bashing, camel riding, traditional Qatari dinner, and cultural performances under the stars. Transportation and dinner included.",
                    Category = "Entertainment",
                    EventType = "Adventure",
                    Venue = "Qatar Desert - Meet at Aptech Qatar",
                    EventDate = DateTime.Now.AddDays(30),
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    OrganizingDepartment = "Student Activities",
                    OrganizerName = "Ms. Fatima Al-Attiyah",
                    OrganizerEmail = "fatima.alattiyah@aptechqatar.com",
                    MaxCapacity = 80,
                    CurrentRegistrations = 60,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 75.00m,
                    CreatedAt = DateTime.Now.AddDays(-18),
                    UpdatedAt = DateTime.Now.AddDays(-12)
                },
                new Event
                {
                    Title = "Graduation Ceremony with Arena Multimedia & Middlesex University",
                    Description = "Celebrate the achievements of our graduating students! Joint graduation ceremony featuring Arena Multimedia and Middlesex University graduates. Special guests, keynote speakers, and recognition of academic excellence. Formal ceremony followed by reception.",
                    Category = "Academic",
                    EventType = "Graduation Ceremony",
                    Venue = "Qatar National Convention Centre - Hall A",
                    EventDate = DateTime.Now.AddDays(35),
                    StartTime = new TimeSpan(16, 0, 0),
                    EndTime = new TimeSpan(21, 0, 0),
                    OrganizingDepartment = "Academic Affairs",
                    OrganizerName = "Dr. Ahmed Al-Suwaidi",
                    OrganizerEmail = "ahmed.alsuwaidi@aptechqatar.com",
                    MaxCapacity = 500,
                    CurrentRegistrations = 400,
                    Status = EventStatus.Approved,
                    IsActive = true,
                    BannerImageUrl = null, // Will use dynamic image based on category/type
                    CertificateFee = 0,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                }
            };

            context.Events.AddRange(events);
            await context.SaveChangesAsync();
        }
    }
}