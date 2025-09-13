namespace EventSphere.Services
{
    public interface IQRCodeService
    {
        Task<string> GenerateQRCodeAsync(int eventId, string userId);
        Task<bool> ValidateQRCodeAsync(string qrCode);
        Task<string> GenerateQRCodeImageAsync(int eventId, string userId);
    }
}

