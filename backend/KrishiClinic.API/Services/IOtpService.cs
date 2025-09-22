namespace KrishiClinic.API.Services
{
    public interface IOtpService
    {
        string GenerateOtp();
        Task<bool> SendOtpAsync(string mobile, string otp);
        bool ValidateOtp(string otp);
    }
}

