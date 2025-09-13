using QRCoder;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;


namespace EventSphere.Services
{
    public class QRCodeService : IQRCodeService
    {
        public async Task<string> GenerateQRCodeAsync(int eventId, string userId)
        {
            var qrData = $"{eventId}:{userId}:{DateTime.Now:yyyyMMddHHmmss}";
            return await Task.FromResult(Convert.ToBase64String(Encoding.UTF8.GetBytes(qrData)));
        }

        public async Task<bool> ValidateQRCodeAsync(string qrCode)
        {
            try
            {
                var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(qrCode));
                var parts = decodedData.Split(':');
                
                if (parts.Length != 3) return false;
                
                // Validate that the parts are valid
                if (!int.TryParse(parts[0], out int eventId)) return false;
                if (string.IsNullOrEmpty(parts[1])) return false;
                if (!DateTime.TryParseExact(parts[2], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime timestamp)) return false;
                
                // Check if QR code is not too old (e.g., within 24 hours)
                if (DateTime.Now - timestamp > TimeSpan.FromHours(24)) return false;
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateQRCodeImageAsync(int eventId, string userId)
        {
            // For now, return the base64 encoded QR data
            // This can be enhanced later with proper image generation
            var qrData = await GenerateQRCodeAsync(eventId, userId);
            return await Task.FromResult(qrData);
        }
    }
}

