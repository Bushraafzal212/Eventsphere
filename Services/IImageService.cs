using EventSphere.Models;

namespace EventSphere.Services
{
    public interface IImageService
    {
        string GetEventImageUrl(Event eventModel);
        string GetDefaultImageForCategory(string category);
        string GetDefaultImageForEventType(string eventType);
        string GetFallbackImage();
    }
}
