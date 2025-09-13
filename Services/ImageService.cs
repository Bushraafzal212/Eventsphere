using EventSphere.Models;

namespace EventSphere.Services
{
    public class ImageService : IImageService
    {
        private readonly Dictionary<string, string> _categoryImages;
        private readonly Dictionary<string, string> _eventTypeImages;

        public ImageService()
        {
            _categoryImages = new Dictionary<string, string>
            {
                ["Cultural"] = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800&h=400&fit=crop",
                ["Academic"] = "https://images.unsplash.com/photo-1521737711867-e3b97375f902?w=800&h=400&fit=crop",
                ["Sports"] = "https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=800&h=400&fit=crop",
                ["Social"] = "https://images.unsplash.com/photo-1511632765486-a01980e01a18?w=800&h=400&fit=crop",
                ["Entertainment"] = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&h=400&fit=crop",
                ["Career"] = "https://images.unsplash.com/photo-1582213782179-e0d53f98f2ca?w=800&h=400&fit=crop"
            };

            _eventTypeImages = new Dictionary<string, string>
            {
                ["Workshop"] = "https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?w=800&h=400&fit=crop",
                ["Competition"] = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=400&fit=crop",
                ["Tournament"] = "https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=800&h=400&fit=crop",
                ["Information Session"] = "https://images.unsplash.com/photo-1521737711867-e3b97375f902?w=800&h=400&fit=crop",
                ["Graduation Ceremony"] = "https://images.unsplash.com/photo-1521737711867-e3b97375f902?w=800&h=400&fit=crop",
                ["Adventure"] = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&h=400&fit=crop",
                ["Ceremony & Job Fair"] = "https://images.unsplash.com/photo-1582213782179-e0d53f98f2ca?w=800&h=400&fit=crop",
                ["Seminar"] = "https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=800&h=400&fit=crop",
                ["Conference"] = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop",
                ["Exhibition"] = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop",
                ["Performance"] = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800&h=400&fit=crop",
                ["Festival"] = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800&h=400&fit=crop",
                ["Networking"] = "https://images.unsplash.com/photo-1511632765486-a01980e01a18?w=800&h=400&fit=crop",
                ["Training"] = "https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?w=800&h=400&fit=crop"
            };
        }

        public string GetEventImageUrl(Event eventModel)
        {
            // If event has a custom banner image, use it
            if (!string.IsNullOrEmpty(eventModel.BannerImageUrl))
            {
                return eventModel.BannerImageUrl;
            }

            // Try to get image based on event type first
            if (!string.IsNullOrEmpty(eventModel.EventType) && 
                _eventTypeImages.ContainsKey(eventModel.EventType))
            {
                return _eventTypeImages[eventModel.EventType];
            }

            // Fall back to category-based image
            if (!string.IsNullOrEmpty(eventModel.Category) && 
                _categoryImages.ContainsKey(eventModel.Category))
            {
                return _categoryImages[eventModel.Category];
            }

            // Final fallback
            return GetFallbackImage();
        }

        public string GetDefaultImageForCategory(string category)
        {
            return _categoryImages.ContainsKey(category) 
                ? _categoryImages[category] 
                : GetFallbackImage();
        }

        public string GetDefaultImageForEventType(string eventType)
        {
            return _eventTypeImages.ContainsKey(eventType) 
                ? _eventTypeImages[eventType] 
                : GetFallbackImage();
        }

        public string GetFallbackImage()
        {
            return "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop";
        }
    }
}
